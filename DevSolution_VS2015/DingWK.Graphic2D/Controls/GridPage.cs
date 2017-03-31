﻿using System.Windows;
using System.Windows.Media;

namespace DingWK.Graphic2D.Controls
{
    public class GridPage : UIElement
    {
        private const int Interval = 5;


        #region Width
        /// <summary>
        /// 
        /// </summary>
        public double Width
        {
            get { return (double)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }
        //
        // Dependency property definition
        //
        private static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(
                nameof(Width),
                typeof(double),
                typeof(GridPage),
                new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion


        #region Heigth
        /// <summary>
        /// 
        /// </summary>
        public double Height
        {
            get { return (double)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }
        //
        // Dependency property definition
        //
        private static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(
                nameof(Height),
                typeof(double),
                typeof(GridPage),
                new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        
        #region Scale
        /// <summary>
        /// 
        /// </summary>
        public double Scale
        {
            get { return (double)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }
        //
        // Dependency property definition
        //
        private static readonly DependencyProperty ScaleProperty =
            DependencyProperty.Register(
                nameof(Scale),
                typeof(double),
                typeof(GridPage),
                new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion

        
        #region GridSize
        /// <summary>
        /// 
        /// </summary>
        public int GridSize
        {
            get { return (int)GetValue(GridSizeProperty); }
            set { SetValue(GridSizeProperty, value); }
        }
        //
        // Dependency property definition
        //
        private static readonly DependencyProperty GridSizeProperty =
            DependencyProperty.Register(
                nameof(GridSize),
                typeof(int),
                typeof(GridPage),
                new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.AffectsRender)
                {
                    CoerceValueCallback = (d, baseValue) =>
                    {
                        int intValue = (int)baseValue;
                        return (intValue < Interval) ? Interval : intValue - intValue % Interval;
                    }
                });
        #endregion


        #region GridBrush
        /// <summary>
        /// 
        /// </summary>
        public Brush GridBrush
        {
            get { return (Brush)GetValue(GridBrushProperty); }
            set { SetValue(GridBrushProperty, value); }
        }
        //
        // Dependency property definition
        //
        private static readonly DependencyProperty GridBrushProperty =
            DependencyProperty.Register(
                nameof(GridBrush),
                typeof(Brush),
                typeof(GridPage),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray), FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion


        #region PageBrush
        /// <summary>
        /// 
        /// </summary>
        public Brush PageBrush
        {
            get { return (Brush)GetValue(PageBrushProperty); }
            set { SetValue(PageBrushProperty, value); }
        }
        //
        // Dependency property definition
        //
        private static readonly DependencyProperty PageBrushProperty =
            DependencyProperty.Register(
                nameof(PageBrush),
                typeof(Brush),
                typeof(GridPage),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White), FrameworkPropertyMetadataOptions.AffectsRender));
        #endregion


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            
            Matrix mtx = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            double dpiFactor = 1 / mtx.M11;
            double delt = dpiFactor / 2;

            double xlen = Width * Scale;
            double ylen = Height * Scale;
                        
            Pen majorPen = new Pen(GridBrush.CloneCurrentValue(), 1 * dpiFactor);
            Brush brush = GridBrush.CloneCurrentValue();
            brush.Opacity = 0.5;
            Pen minorPen = new Pen(brush, 1 * dpiFactor);

            if (majorPen.CanFreeze) majorPen.Freeze();
            if (minorPen.CanFreeze) minorPen.Freeze();

            GuidelineSet guidelineSet = new GuidelineSet();
            guidelineSet.GuidelinesX.Add(0 - delt);
            guidelineSet.GuidelinesX.Add(xlen - delt);

            guidelineSet.GuidelinesY.Add(0 - delt);
            guidelineSet.GuidelinesY.Add(ylen - delt);

            drawingContext.PushGuidelineSet(guidelineSet);
            drawingContext.DrawRectangle(null, minorPen, new Rect(0, 0, xlen, ylen));
            drawingContext.Pop();

            LineGeometry lgx = new LineGeometry(new Point(0, 0), new Point(0, ylen));
            LineGeometry lgy = new LineGeometry(new Point(0, 0), new Point(xlen, 0));
            if (lgx.CanFreeze) lgx.Freeze();
            if (lgy.CanFreeze) lgy.Freeze();

            drawingContext.DrawRectangle(PageBrush, null, new Rect(0, 0, xlen, ylen));

            for (double x = 0; x <= Width; x += GridSize)
            {
                GuidelineSet gridGuidelines = new GuidelineSet();
                gridGuidelines.GuidelinesX.Add(x * Scale - delt);

                drawingContext.PushGuidelineSet(gridGuidelines);
                drawingContext.PushTransform(new TranslateTransform(x * Scale, 0));
                drawingContext.DrawGeometry(null, (x / GridSize) % Interval == 0 ? majorPen : minorPen, lgx);
                drawingContext.Pop();
                drawingContext.Pop();
            }

            for (double y = 0; y <= Height; y += GridSize)
            {
                GuidelineSet gridGuidelines = new GuidelineSet();
                gridGuidelines.GuidelinesY.Add(y * Scale - delt);

                drawingContext.PushGuidelineSet(gridGuidelines);
                drawingContext.PushTransform(new TranslateTransform(0, y * Scale));
                drawingContext.DrawGeometry(null, (y / GridSize) % Interval == 0 ? majorPen : minorPen, lgy);
                drawingContext.Pop();
                drawingContext.Pop();
            }
            
        }


    }
}