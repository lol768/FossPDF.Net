namespace FossPDF.Infrastructure
{
    public interface IRenderingCanvas
    {
        void BeginDocument();
        void EndDocument();
        
        void BeginPage(Size size);
        void EndPage();
    }
}
