/*using System;
using System.Linq;

namespace CSSUI.Platform
{
    /// <summary>
    /// Implements generic functions concerning DragDrop operations
    /// </summary>
    public static class DragNDrop
    {
        /// <summary>
        /// Specifys custom drag-drop data formats for the program
        /// </summary>
        public static class CustFormat
        {
            /// <summary>
            /// Used to store where an image being drag-dropped is coming from when the operation is internal.
            /// </summary>
            public const string ImageSource = "IMAGE_SOURCE";
            /// <summary>
            /// Used to specify an internal operation moving image hashes around.
            /// </summary>
            public const string ImageHashes = "IMAGE_HASHES";
        }

        /// <summary>
        /// Corresponds to DragEventArgs.KeyState values
        /// </summary>
        [Flags]
        public enum KeyState
        {
            LeftMouse = 1,
            RightMouse = 2,
            ShiftKey = 4,
            CtrlKey = 8,
            MiddleMouse = 16,
            AltKey = 32,
        }

        /// <summary>
        /// Returns <c>TRUE</c> is the passed in DragDrop data is acceptable for inserting/moving items within the program.
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static bool Is_Acceptable_DragDrop_Data(IDataObject Data)
        {
            if (Data.GetDataPresent(DataFormats.Text, false))
            {
                string filename = (string)Data.GetData(DataFormats.Text);
                return true;
            }

            if (Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                string[] data = (string[])Data.GetData(DataFormats.FileDrop);
                if (data != null)
                {
                    foreach (string filename in data)
                    {
                        string ext = System.IO.Path.GetExtension(filename).ToLower();
                        if (ImgUtil.Check_Is_Supported_Image_Extension(ext))
                        {
                            return true;
                        }
                    }
                }
            }

            if (Data.GetDataPresent(CustFormat.ImageHashes, false))
            {
                long[] hashes = (long[])Data.GetData(CustFormat.ImageHashes);
                if (hashes != null)
                {
                    if (hashes.Length > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines which kind of drag-drop effects are valid for an item depending on if it is an internal item or one that must be downloaded.
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static DragDropEffects Get_Allowed_DragDrop_Effects(IDataObject Data)
        {
            if (Data.GetDataPresent(DataFormats.Text, false))// Usually this is going to be a link to an image online
            {
                string filename = (string)Data.GetData(DataFormats.Text);
                return (DragDropEffects.Scroll | DragDropEffects.Copy);// Copy means we are importing it fresh
            }

            if (Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                string[] data = (string[])Data.GetData(DataFormats.FileDrop);
                if (data != null)
                {
                    foreach (string filename in data)
                    {
                        string ext = System.IO.Path.GetExtension(filename).ToLower();
                        if (ImgUtil.Check_Is_Supported_Image_Extension(ext))
                        {
                            return (DragDropEffects.Scroll | DragDropEffects.Copy);// We only need one image to be valid for importing before we allow an operation
                        }
                    }
                }

                return DragDropEffects.None;
            }

            if (Data.GetDataPresent(CustFormat.ImageHashes, false))
            {
                long[] hashes = (long[])Data.GetData(CustFormat.ImageHashes);
                if (hashes != null)
                {
                    if (hashes.Length > 0)
                    {
                        return (DragDropEffects.Scroll | DragDropEffects.Copy | DragDropEffects.Move);// Link means the image was added to an additional folder, Move means we removed it from one folder and put it into another.
                    }
                }
            }

            return DragDropEffects.None;
        }

        /// <summary>
        /// Returns TRUE if a DragDrop operation would only involve internal data.
        /// </summary>
        public static bool Is_Internal_Action(IDataObject Data)
        {
            if (Data.GetDataPresent(CustFormat.ImageHashes, false))
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Performs drag-drop logic targeted at a specific library folder.
        /// </summary>
        /// <param name="dest">Folder to drag-drop to</param>
        /// <param name="source">Folder being dragged from</param>
        public static void DragDrop_To_Location(string dest, string source, DragEventArgs e)
        {
            DragNDrop.KeyState kp = (DragNDrop.KeyState)e.KeyState;
            bool CTRL = ((kp & DragNDrop.KeyState.CtrlKey) == DragNDrop.KeyState.CtrlKey) || (((int)e.KeyState & (int)KeyState.CtrlKey) == (int)KeyState.CtrlKey);
            bool SHIFT = ((kp & DragNDrop.KeyState.ShiftKey) == DragNDrop.KeyState.ShiftKey) || (((int)e.KeyState & (int)KeyState.ShiftKey) == (int)KeyState.ShiftKey);

            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {// Dragging files from disk to the library
                string[] data = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (data != null)
                {
                    foreach (string FN in data)
                    {
                        Queue_Image_Download(FN, dest, CTRL);
                    }
                }
            }
            else if (e.Data.GetDataPresent((DataFormats.Text)))
            {// Dragging links from browser to library
                string filename = (string)e.Data.GetData(DataFormats.Text);
                if (!string.IsNullOrEmpty(filename))
                {
                    Queue_Image_Download(filename, dest, CTRL);
                }
            }
            else if (e.Data.GetDataPresent(CustFormat.ImageHashes, false))
            {// User is dragging images to new folders within the program
                long[] hashes = (long[])e.Data.GetData(CustFormat.ImageHashes);
                if (hashes != null)
                {
                    if (hashes.Count() > 0)
                    {
                        string dname = dest.Split("\\".ToCharArray()).Last();
                        Program.Notify(string.Format("Moving {0} images to \"{1}\"", hashes.Count(), dname));
                        if (CTRL || SHIFT)// We are adding the images to an ADDITIONAL folder
                        {
                            foreach (long hash in hashes)
                            {
                                Program.VFS.vf_add_image_location(hash, dest);
                            }
                        }
                        else// We are moving the images out of one folder and into another
                        {
                            if (!e.Data.GetDataPresent(CustFormat.ImageSource, false))
                            {
                                throw new ArgumentNullException("No source for the image hashes was specified during the drag-drop operation!");
                            }
                            else
                            {
                                string Src = (string)e.Data.GetData(CustFormat.ImageSource, false);
                                foreach (long hash in hashes)
                                {
                                    Program.VFS.vf_move_image(hash, Src, dest);
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
*/