//#define USE_PARALLEL

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CssUI.Difference
{
    public class DiffEngine<T> where T : class//, IEquatable<T>
    {
        #region Properties
        #endregion

        #region Constructors
        public DiffEngine()
        {

        }
        #endregion

        #region Transform
        /// <summary>
        /// Compiles a list of positions and data to insert at said positions in order to make <paramref name="OldData"/> match <paramref name="NewData"/>
        /// </summary>
        /// <returns></returns>
        public LinkedList<DiffData<T>> Compile_Transformations(DataConsumer<T> OldData, DataConsumer<T> NewData, out int outChange)
        {
            if (OldData is null) throw new ArgumentNullException(nameof(OldData));
            if (NewData is null) throw new ArgumentNullException(nameof(NewData));
            Contract.EndContractBlock();

            /* Get all of the places where we have changed */
            LinkedList<DiffNode<T>> Diffs = Get_Difference(OldData, NewData);

            /* We want a series of commands that ONLY replace text which has changed BUT which does not skip CSI commands that may apply to those differences AND which do not include redundant CSI commands */
            int Balance = 0;// Tracks the total balance of data added vs removed
            var RetList = new LinkedList<DiffData<T>>();
            LinkedListNode<DiffNode<T>> node = Diffs.First;

            while (node != null)
            {
                DiffNode<T> current = node.Value;
                /* In order to account for the total changes which have occured we need to ensure that any data which will be overwritten which isn't supposed to be replaced will correctly be updated after overwriting */
                switch (current.Type)
                {
                    case EDiffAction.Modify:
                        {
                            /* This data was already present, it should be invisible to our change count */
                            RetList.AddLast(new DiffData<T>(EDiffAction.Modify, current.StartOffset, NewData.AsMemory().Slice(current.StartOffset, current.Length)));
                        }
                        break;
                    case EDiffAction.Insertion:
                        {/* In the case of inserted data we should insert all the data that WAS at the start of this diff and before the next */

                            RetList.AddLast(new DiffData<T>(EDiffAction.None, current.StartOffset, NewData.AsMemory().Slice(current.StartOffset, current.Length)));
                            Balance += current.Length;
                            var next = node.Next;
                            if (next is object)
                            {
                                /* The length of data we need to repeat is whatever was between our insertion pos and the start of the next diff */
                                var repeat_length = next.Value.StartOffset - current.StartOffset;
                                /* This data was already present, it should be invisible to our change count */
                                RetList.AddLast(new DiffData<T>(EDiffAction.Insertion, current.EndOffset, NewData.AsMemory().Slice(current.EndOffset, repeat_length)));/* We read from this diff's end because in the new buffer the repeated text is after this diff */
                            }
                            else/* No next, insert all needed data */
                            {
                                var repeat_length = (OldData?.Length ?? 0) - current.StartOffset;
                                RetList.AddLast(new DiffData<T>(EDiffAction.Insertion, current.EndOffset, NewData.AsMemory().Slice(current.EndOffset, repeat_length)));/* We read from this diff's end because in the new buffer the repeated text is after this diff */
                            }
                        }
                        break;
                    case EDiffAction.Removal:
                        {/* For removals we just want to track how much data was removed */
                            Balance -= current.Length;
                        }
                        break;
                }

                node = node.Next;
            }

            outChange = Balance;
            return RetList;
        }
        #endregion

        #region Compiling
        /// <summary>
        /// Compiles a list of data-nodes which represent sequences of data and indicates what kind of change happened, if any.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LinkedList<DiffData<T>> Compile(in T[] Old, in T[] New)
        {
            var oldStream = new DataConsumer<T>(Old);
            var newStream = new DataConsumer<T>(New);
            return Compile(oldStream, newStream);
        }

        /// <summary>
        /// Compiles a list of data-nodes which represent sequences of data and indicates what kind of change happened, if any.
        /// </summary>
        public LinkedList<DiffData<T>> Compile(DataConsumer<T> OldData, DataConsumer<T> NewData)
        {
            if (OldData is null) throw new ArgumentNullException(nameof(OldData));
            if (NewData is null) throw new ArgumentNullException(nameof(NewData));
            Contract.EndContractBlock();

            /* Get all of the places where we have changed */
            LinkedList<DiffNode<T>> Diffs = Get_Difference(OldData, NewData);

            /* We want a series of commands that ONLY replace text which has changed BUT which does not skip CSI commands that may apply to those differences AND which do not include redundant CSI commands */
            var RetList = new LinkedList<DiffData<T>>();
            LinkedListNode<DiffNode<T>> node = Diffs.First;

            while (node is object)
            {
                DiffNode<T> current = node.Value;
                /* Add a node to contain the data that came before this node and after the last node */
                if (current.StartOffset > 0)
                {
                    int start = 0;
                    int end = current.StartOffset;
                    // If there was a previous node then our starting point should be it's ending point
                    if (node.Previous is object)
                        start = node.Previous.Value.EndOffset;

                    int len = (end - start);
                    if (len > 0)
                    {
                        RetList.AddLast(new DiffData<T>(EDiffAction.None, start, NewData.AsMemory().Slice(start, len)));
                    }
                }

                /* Now create a node to represent this change */
                RetList.AddLast(new DiffData<T>(current.Type, current.StartOffset, current.Data));

                node = node.Next;
            }

            return RetList;
        }
        #endregion

        #region Differencing
        /// <summary>
        /// Compiles a list of differences between the <paramref name="New"/> and <paramref name="Old"/> data consumers
        /// </summary>
        /// <param name="Old"></param>
        /// <param name="New"></param>
        /// <returns>List of start/end ranges</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LinkedList<DiffNode<T>> Get_Difference(in T[] Old, in T[] New)
        {
            var oldStream = new DataConsumer<T>(Old);
            var newStream = new DataConsumer<T>(New);
            return Get_Difference(oldStream, newStream);
        }

        /// <summary>
        /// Compiles a list of differences between the <paramref name="New"/> and <paramref name="Old"/> data consumers
        /// </summary>
        /// <param name="Old"></param>
        /// <param name="New"></param>
        /// <returns>List of start/end ranges</returns>
        public LinkedList<DiffNode<T>> Get_Difference(DataConsumer<T> Old, DataConsumer<T> New)
        {
            if (Old.IsNullOrEmpty() && New.IsNullOrEmpty())
            {
                return new LinkedList<DiffNode<T>>();
            }
            else if (Old.IsNullOrEmpty() && !New.IsNullOrEmpty())
            {
                return new LinkedList<DiffNode<T>>(new DiffNode<T>[] { new DiffNode<T>(New.AsMemory(), 0, New.Length, EDiffAction.Insertion) });
            }
            else if (New.IsNullOrEmpty() && !Old.IsNullOrEmpty())
            {
                return new LinkedList<DiffNode<T>>(new DiffNode<T>[] { new DiffNode<T>(Old.AsMemory(), 0, Old.Length, EDiffAction.Insertion) });
            }

            var Chunks = new LinkedList<DiffNode<T>>();
            while (!Old.atEnd || !New.atEnd)
            {
                var diff = Consume(Old, New);
                if (diff is null || diff.Length <= 0)
                {/* No more diffs available */
                    return Chunks;
                }
                /* Progress the applicable consumer by the difference ammount */
                switch (diff.Type)
                {
                    case EDiffAction.Insertion:
                        {
                            New.Seek(diff.EndOffset);
                            //New.Consume(diff.Length);
                        }
                        break;
                    case EDiffAction.Removal:
                        {
                            Old.Seek(diff.EndOffset);
                            //Old.Consume(diff.Length);
                        }
                        break;
                    case EDiffAction.None:
                    case EDiffAction.Modify:
                        {
                            Old.Consume(diff.Length);
                            New.Consume(diff.Length);
                        }
                        break;
                    default:
                        throw new NotImplementedException($"Handling for {nameof(EDiffAction)} \"{diff.Type}\" has not been implemented!");
                }

                Chunks.AddLast(diff);
            }

            return Chunks;
        }
        #endregion

        #region Consuming
        /// <summary>
        /// Locates the next difference point between the <paramref name="New"/> and <paramref name="Old"/> data consumers
        /// </summary>
        /// <param name="Old"></param>
        /// <param name="New"></param>
        /// <returns></returns>
        private DiffNode<T> Consume(DataConsumer<T> Old, DataConsumer<T> New)
        {
            /* Scan both streams until we either find a spot ahead in one that matches the current spot in the other OR we hit the end of the stream */
            T nOld = Old.Next;
            T nNew = New.Next;
            DiffNode<T> addDiff = null;
            DiffNode<T> rmvDiff = null;

            /* To start off we need to ensure both streams are positioned at the next mismatch */
            if (Scan_Lockstep_Mismatch(Old, New, out int outOldPos, out int outNewPos))
            {
                Old.Seek(outOldPos);
                New.Seek(outNewPos);
            }
            else
            {
                /* If no mismatch was found then we are done! */
                if (!New.atEnd)
                {
                    return new DiffNode<T>(New.AsMemory(), New.Position, New.Length, EDiffAction.None);
                }
                return null;
            }

            /* Now we are at an index where a mismatch occurs between the data. 
             * Next we need to figure out at what index the data starts to match again.
             * The value we need to match for each is the current value of the opposing data stream
             * This is because we are constructing a POTENTAL insertion difference, 
             * we compare the two difference nodes afterwards to decide which is accurate.
             */

#if USE_PARALLEL
            var CancelToken = new CancellationTokenSource();
            Parallel.Invoke(new ParallelOptions() { CancellationToken = CancelToken.Token },
                () =>/* Scan for addition */
                {/* Find the next unique in New that matches Old's current */
#endif
            if (Old.atEnd && !New.atEnd)
            {// Old buffer has no more data so any remaining data in the New buffer was added
                addDiff = new DiffNode<T>(New.AsMemory(), New.Position, New.Length - 1, EDiffAction.Insertion);/* Insertion at end */
            }
            else if (nNew is object)
            {
                if (Scan_Match(New, Old.Next, out int endIndex))
                {
                    addDiff = new DiffNode<T>(New.AsMemory(), New.Position, endIndex, EDiffAction.Insertion);
                }
            }
#if USE_PARALLEL
                },
                () =>/* Scan for removal */
                {/* Find next unique in Old matching New's current */
#endif
            if (New.atEnd && !Old.atEnd)
            {// New buffer has no more data, so all of the data remaining within the Old buffer was removed
                rmvDiff = new DiffNode<T>(Old.AsMemory(), Old.Position, Old.Length, EDiffAction.Removal);/* Removal at end */
            }
            else if (nOld is object)
            {
                if (Scan_Match(Old, New.Next, out int endIndex))
                {
                    rmvDiff = new DiffNode<T>(Old.AsMemory(), Old.Position, endIndex, EDiffAction.Removal);
                }
            }
#if USE_PARALLEL
                }
            );
#endif

            /* If nothing was found then the change must be a pure modification so we lockstep ahead in both streams to find the next spot where they both match again */
            if (addDiff is null && rmvDiff is null)
            {
                if (Scan_Lockstep_Match(Old, New, out int _, out int pos))
                {
                    return new DiffNode<T>(New.AsMemory(), New.Position, pos, EDiffAction.Modify);
                }
                else
                {
                    if (!New.atEnd)
                    {
                        return new DiffNode<T>(New.AsMemory(), New.Position, New.Length, EDiffAction.None);
                    }
                    return null;
                }
            }
            else if (addDiff is object && rmvDiff is object)/* Else find the shorter of the two diffs and return that one */
            {
                if (addDiff.Length < rmvDiff.Length)
                {
                    return addDiff;
                }
                else
                {
                    return rmvDiff;
                }
            }
            else if (addDiff is object)
            {
                return addDiff;
            }
            else if (rmvDiff is object)
            {
                return rmvDiff;
            }

            return null;
        }
        #endregion

        #region Scan
        /// <summary>
        /// Lockstep searches the two given streams and returns the next location where they match
        /// </summary>
        /// <param name="Offset">Offset to begin searching from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Scan_Lockstep_Match(DataConsumer<T> Old, DataConsumer<T> New, out int outOldPos, out int outNewPos, int Offset = 0)
        {
            if (Old is null) throw new ArgumentNullException(nameof(Old));
            if (New is null) throw new ArgumentNullException(nameof(New));
            Contract.EndContractBlock();

            int oPos = Offset + Old.Position;
            int nPos = Offset + New.Position;
            while (oPos < Old.Length && nPos < New.Length)
            {
                var ov = Old.Get(oPos);
                var nv = New.Get(nPos);
                if (ov.Equals(nv))
                {
                    outOldPos = oPos;
                    outNewPos = nPos;
                    return true;
                }
                oPos++;
                nPos++;
            }

            outOldPos = -1;
            outNewPos = -1;
            return false;
        }

        /// <summary>
        /// Lockstep searches the two given streams returns the next location where they don't match
        /// </summary>
        /// <param name="Offset">Offset to begin searching from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Scan_Lockstep_Mismatch(DataConsumer<T> Old, DataConsumer<T> New, out int outOldPos, out int outNewPos, int Offset = 0)
        {
            if (Old is null) throw new ArgumentNullException(nameof(Old));
            if (New is null) throw new ArgumentNullException(nameof(New));
            Contract.EndContractBlock();

            int oPos = Offset + Old.Position;
            int nPos = Offset + New.Position;
            while (oPos < Old.Length && nPos < New.Length)
            {
                var ov = Old.Get(oPos);
                var nv = New.Get(nPos);
                if (!ov.Equals(nv))
                {
                    outOldPos = oPos;
                    outNewPos = nPos;
                    return true;
                }
                oPos++;
                nPos++;
            }

            outOldPos = -1;
            outNewPos = -1;
            return false;
        }


        /// <summary>
        /// Searches the given stream and returns the next location where its value matches the search value
        /// </summary>
        /// <param name="Offset">Offset to begin searching from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Scan_Match(DataConsumer<T> Left, DataConsumer<T> Right, out int outIndex, int Offset = 0)
        {
            if (Left is null) throw new ArgumentNullException(nameof(Left));
            if (Right is null) throw new ArgumentNullException(nameof(Right));
            Contract.EndContractBlock();

            int leftPos = Offset + Left.Position;
            int rightPos = Offset + Right.Position;
            while (leftPos < Left.Length && rightPos < Right.Length)
            {
                var leftValue = Left.Get(leftPos);
                var rightValue = Right.Get(rightPos);
                if (leftValue.Equals(rightValue))
                {
                    outIndex = leftPos;
                    return true;
                }
                leftPos++;
                rightPos++;
            }

            outIndex = -1;
            return false;
        }

        /// <summary>
        /// Searches the given stream and returns the next location where its value doesn't match the search value
        /// </summary>
        /// <param name="Offset">Offset to begin searching from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Scan_Mismatch(DataConsumer<T> Left, DataConsumer<T> Right, out int outIndex, int Offset = 0)
        {
            if (Left is null) throw new ArgumentNullException(nameof(Left));
            if (Right is null) throw new ArgumentNullException(nameof(Right));
            Contract.EndContractBlock();

            int leftPos = Offset + Left.Position;
            int rightPos = Offset + Right.Position;
            while (leftPos < Left.Length && rightPos < Right.Length)
            {
                var leftValue = Left.Get(leftPos);
                var rightValue = Right.Get(rightPos);
                if (!leftValue.Equals(rightValue))
                {
                    outIndex = leftPos;
                    return true;
                }
                leftPos++;
                rightPos++;
            }

            outIndex = -1;
            return false;
        }

        /// <summary>
        /// Searches the given stream and returns the next location where its value matches the search value
        /// </summary>
        /// <param name="Offset">Offset to begin searching from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Scan_Match(DataConsumer<T> Stream, T Match, out int outIndex, int Offset = 0)
        {
            if (Stream is null) throw new ArgumentNullException(nameof(Stream));
            Contract.EndContractBlock();

            int Pos = Offset + Stream.Position;
            while (Pos < Stream.Length)
            {
                if (Stream.Get(Pos).Equals(Match))
                {
                    outIndex = Pos;
                    return true;
                }
                Pos++;
            }
            outIndex = -1;
            return false;
        }

        /// <summary>
        /// Searches the given stream and returns the next location where its value doesn't match the search value
        /// </summary>
        /// <param name="Offset">Offset to begin searching from</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Scan_Mismatch(DataConsumer<T> Stream, T Match, out int outIndex, int Offset = 0)
        {
            if (Stream is null) throw new ArgumentNullException(nameof(Stream));
            Contract.EndContractBlock();

            int Pos = Offset + Stream.Position;
            while (Pos < Stream.Length)
            {
                if (!Stream.Get(Pos).Equals(Match))
                {
                    outIndex = Pos;
                    return true;
                }
                Pos++;
            }
            outIndex = -1;
            return false;
        }
        #endregion

        #region Formatting
        public string Format(IEnumerable<DiffNode<T>> Nodes)
        {
            if (Nodes is null) throw new ArgumentNullException(nameof(Nodes));
            Contract.EndContractBlock();

            StringBuilder buf = new StringBuilder();
            foreach (var node in Nodes)
            {
                buf.AppendLine(node.ToString());
            }

            return buf.ToString();
        }

        public string FormatAsHTML(IEnumerable<DiffData<T>> Nodes)
        {
            StringBuilder RetVal = new StringBuilder();

            RetVal.AppendLine(@"<html>");
            RetVal.AppendLine(@"<head></head>");
            RetVal.AppendLine(@"<body>");
            RetVal.AppendLine(@"<style>
text { padding: 0px 0px; }
text.diff-type-none { color: #333; }
text.diff-type-insert { color: #F0F0F0; background-color: #37BF28; }
text.diff-type-remove { color: #F0F0F0; background-color: #FF4A3C; }
text.diff-type-modify { color: #F0F0F0; background-color: #3967FF; }
</style>");
            RetVal.AppendLine("<p>");

            if (Nodes is object)
            {
                foreach (DiffData<T> Node in Nodes)
                {
                    RetVal.Append("<text ");
                    switch (Node.Type)
                    {
                        case EDiffAction.None:
                            {
                                RetVal.Append(@"class=""diff-type-none""");
                            }
                            break;
                        case EDiffAction.Insertion:
                            {
                                RetVal.Append(@"class=""diff-type-insert""");
                            }
                            break;
                        case EDiffAction.Removal:
                            {
                                RetVal.Append(@"class=""diff-type-remove""");
                            }
                            break;
                        case EDiffAction.Modify:
                            {
                                RetVal.Append(@"class=""diff-type-modify""");
                            }
                            break;
                    }

                    RetVal.Append(">");
                    RetVal.Append(Node.ToString());
                    RetVal.Append("</text>");
                }
            }
            else
            {
                RetVal.AppendLine(@"<text style=""color: #555; font-weight: bold;"">Nothing Here!</text>");
            }

            RetVal.AppendLine("</p>");
            RetVal.AppendLine("</body>");
            RetVal.AppendLine("</html>");
            return RetVal.ToString();
        }

        public bool DisplayHTML(IEnumerable<DiffData<T>> Nodes)
        {
            string htmlStr = FormatAsHTML(Nodes);
            string base64Str = Convert.ToBase64String(Encoding.UTF8.GetBytes(htmlStr));
            string args = @$"data:text/html;charset=utf-8;base64,{base64Str}";

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = @$"{Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)}\Google\Chrome\Application\chrome.exe",
                Arguments = args,
                LoadUserProfile = true,
            };
            try
            {
                Process.Start(startInfo);
                return true;
            }
            catch
            {
                return false;
            }

        }
        #endregion
    }
}
