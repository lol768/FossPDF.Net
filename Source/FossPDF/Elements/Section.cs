using FossPDF.Infrastructure;

namespace FossPDF.Elements
{
    internal class Section : ContainerElement, IStateResettable
    {
        public string LocationName { get; set; }
        private bool IsRendered { get; set; }
        
        public void ResetState()
        {
            IsRendered = false;
        }
        
        internal override void Draw(Size availableSpace)
        {
            if (!IsRendered)
            {
                Canvas.DrawSection(LocationName);
                IsRendered = true;
            }
            
            PageContext.SetSectionPage(LocationName);
            base.Draw(availableSpace);
        }
    }
}
