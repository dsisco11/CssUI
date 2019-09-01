
namespace CssUI.HTML
{
    public class FileDataRequest : DataRequest
    {
        #region Constructor
        public FileDataRequest(string currentURL) : base(currentURL)
        {
        }
        #endregion


        public override void Fetch()
        {
            if (Data != null) return;

            try
            {
                Data = new System.IO.MemoryStream();
                using (var file = System.IO.File.OpenRead(currentURL))
                {
                    const int CHUNK_SIZE = 1024;// 1kb
                    byte[] buffer = new byte[CHUNK_SIZE];
                    Data.SetLength(file.Length);

                    while (file.Position < file.Length)
                    {
                        file.Read(Data.GetBuffer(), 0, CHUNK_SIZE);
                    }
                }

                State = DOM.Enums.EDataRequestState.CompletelyAvailable;
            }
            catch
            {
                State = DOM.Enums.EDataRequestState.Broken;
            }
        }
    }
}
