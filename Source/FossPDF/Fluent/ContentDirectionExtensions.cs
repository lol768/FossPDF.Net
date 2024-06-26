﻿using FossPDF.Elements;
using FossPDF.Infrastructure;

namespace FossPDF.Fluent
{
    public static class ContentDirectionExtensions
    {
        internal static IContainer ContentDirection(this IContainer element, ContentDirection direction)
        {
            return element.Element(new ContentDirectionSetter
            {
                ContentDirection = direction
            });
        }
        
        public static IContainer ContentFromLeftToRight(this IContainer element)
        {
            return element.ContentDirection(Infrastructure.ContentDirection.LeftToRight);
        }
        
        public static IContainer ContentFromRightToLeft(this IContainer element)
        {
            return element.ContentDirection(Infrastructure.ContentDirection.RightToLeft);
        }
    }
}
