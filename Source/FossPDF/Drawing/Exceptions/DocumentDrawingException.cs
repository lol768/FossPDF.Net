﻿using System;

namespace FossPDF.Drawing.Exceptions
{
    public class DocumentDrawingException : Exception
    {
        internal DocumentDrawingException(string message) : base(message)
        {
            
        }
        
        internal DocumentDrawingException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}
