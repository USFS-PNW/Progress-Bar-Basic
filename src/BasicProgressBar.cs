﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;


namespace ProgressBarBasic
{

    #region ProgressBarBasic

    /// <summary>
    /// A basic vertical/horizontal ProgressBar
    /// </summary>
    public partial class ProgressBarBasic : Control
    {
        /// <summary>
        /// Value changed event handler delegate
        /// </summary>
        /// <param name="sender">the ProgressBar</param>
        /// <param name="e">ValueChangedEventArgs</param>
        public delegate void ValueChangedEventHandler(object sender, ValueChangedEventArgs e);

        /// <summary>
        /// Determines how the text on the progressbar is shown.
        /// <para>None - No text is drawn</para>
        /// <para>Percentage - The percentage done is shown</para>
        /// <para>Text - Draws the text assosciated with the control</para>
        /// <para>Value - The current Value is shown</para>
        /// <para>ValueOverMaximum - The current Value shown with the Maximum value</para>
        /// </summary>
        public enum TextStyleType
        {
            None,
            Percentage,
            Text,
            Value,
            ValueOverMaximum
        };

        private int minimum = 0;
        private int maximum = 100;
        private int currentValue = 25;

        private Orientation orientation = Orientation.Vertical;

        private Color barColor = Color.DodgerBlue;
        private Color borderColor = Color.Black;
        private int borderThickness = 2;

        private TextStyleType textStyle = TextStyleType.Value;
        private TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis;
        private Color textColor = Color.Black;

        private CompositingMode compositingMode = CompositingMode.SourceOver;
        private CompositingQuality compositingQuality = CompositingQuality.HighQuality;
        private InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic;
        private PixelOffsetMode pixelOffsetMode = PixelOffsetMode.HighQuality;
        private SmoothingMode smoothingMode = SmoothingMode.HighQuality;

        private bool hasErrors = false;
        private string errorLog = null;

        /// <summary>
        /// ProgressBarBasic initialization
        /// </summary>
        public ProgressBarBasic()
        {
            
            this.SetStyle
                (
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.SupportsTransparentBackColor,
                true
                );
            
            this.ForeColor = barColor;
            this.BackColor = Color.DarkGray;
            this.Font = new Font("Consolas", 10.25f);
			OnForeColorChanged(EventArgs.Empty);

        }

        
        /// <summary>
        /// Occurs when the ProgressBar's Value changed.
        /// </summary>
        [Category("Action"),
        Description("Occurs when the ProgressBar's Value changed")]
        public event ValueChangedEventHandler ValueChanged;

        /// <summary>
        /// Occurs when the ProgressBar's value has changed.
        /// </summary>
        /// <param name="e">ValueChangedEventArgs</param>
        protected virtual void OnValueChanged(ValueChangedEventArgs e)
        {
            // Raise the event
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        /// <summary>
        /// Overrides the OnForeColorChanged event to set the current bar color to the ForeColor.
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected override void OnForeColorChanged(EventArgs e)
        {
            barColor = ForeColor;
            base.OnForeColorChanged(e);
        }

        /// <summary>
        /// The maximum value.
        /// </summary>
        [Description("The maximum value."),
        DefaultValue(100)]
        public int Maximum
        {
            get { return maximum; }
            set
            {
                maximum = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// The minimum value.
        /// </summary>
        [Description("The minimum value."),
        DefaultValue(0)]
        public int Minimum
        {
            get { return minimum; }
            set
            {
                minimum = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// The current value.
        /// <para>Note:</para>
        /// <para>If the value is less than the Minimum, the value is set to the Minimum</para>
        /// <para>If the value is greater than the Maximum, the value is set to the Maximum</para>
        /// </summary>
        [Description("The current value."),
        DefaultValue(25)]
        public int Value
        {
            get { return currentValue; }
            set
            {
                if (value >= minimum && value <= maximum)
                {
                    currentValue = value;
                }
                else if (value > maximum)
                {
                    currentValue = maximum;
                }
                else if (value < minimum)
                {
                    currentValue = minimum;
                }

                this.Invalidate();

                OnValueChanged(new ValueChangedEventArgs(currentValue));
            }
        }

        /// <summary>
        /// The border color.
        /// <para>If set to Transparent, no border is drawn.</para>
        /// </summary>
        [Description("The border color."),
        DefaultValue(typeof(Color), "Black")]
        public Color BorderColor
        {
            get { return borderColor; }
            set 
            { 
                borderColor = value; 
                this.Invalidate(); 
            }
        }

        /// <summary>
        /// The border thickness.
        /// <para>If set to 0, no border is drawn.</para>
        /// </summary>
        [Description("The border thickness"),
        DefaultValue(2)]
        public int BorderThickness
        {
            get { return borderThickness; }
            set
            {
                borderThickness = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// The ProgressBar oritentation.
        /// </summary>
        [Description("The ProgressBar oritentation."),
        DefaultValue(typeof(Orientation),"Vertical")]
        public Orientation Orientation
        {
            get { return orientation; }
            set
            {
                orientation = value;
                this.Invalidate();
            }
        }
        
        /// <summary>
        /// The color of the text drawn on the ProgressBar.
        /// <para>If set to transparent, no text is drawn.</para>
        /// </summary>
        [Description("The color of the text drawn on the ProgressBar."),
        DefaultValue(typeof(Color), "Black")]
        public Color TextColor
        {
            get { return textColor; }
            set
            {
                textColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// The way the text on the ProgressBar is drawn.
        /// </summary>
        [Description("The way the text on the ProgressBar is drawn."),
        DefaultValue(typeof(TextStyleType), "Value")]
        public TextStyleType TextStyle
        {
            get { return textStyle; }
            set
            {
                textStyle = value;
                this.Invalidate();
            }
        }

        


        #region Smoothing properties

        /// <summary>
        /// The ProgressBar's Graphic's CompositingMode.
        /// </summary>
        [Description("The ProgressBar's Graphic's CompositingMode."),
        DefaultValue(typeof(CompositingMode), "SourceOver")]
        public CompositingMode CompositingMode
        {
            get { return compositingMode; }
            set 
            { 
                compositingMode = value; 
                this.Invalidate(); 
            }
        }
        /// <summary>
        /// The ProgressBar's Graphic's CompositingQuality.
        /// </summary>
        [Description("The ProgressBar's Graphic's CompositingQuality."),
        DefaultValue(typeof(CompositingQuality), "HighQuality")]
        public CompositingQuality CompositingQuality
        {
            get { return compositingQuality; }
            set
            {
                if (value != CompositingQuality.Invalid)
                {
                    compositingQuality = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// The ProgressBar's Graphic's InterpolationMode.
        /// </summary>
        [Description("The ProgressBar's Graphic's InterpolationMode."),
        DefaultValue(typeof(InterpolationMode), "HighQualityBicubic")]
        public InterpolationMode InterpolationMode
        {
            get { return interpolationMode; }
            set
            {
                if (value != InterpolationMode.Invalid)
                {
                    interpolationMode = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// The ProgressBar's Graphic's PixelOffsetMode.
        /// </summary>
        [Description("The ProgressBar's Graphic's PixelOffsetMode."),
        DefaultValue(typeof(PixelOffsetMode), "HighQuality")]
        public PixelOffsetMode PixelOffsetMode
        {
            get { return pixelOffsetMode; }
            set
            {
                if (value != PixelOffsetMode.Invalid)
                {
                    pixelOffsetMode = value;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// The ProgressBar's Graphic's SmoothingMode.
        /// </summary>
        [Description("The ProgressBar's Graphic's SmoothingMode."),
        DefaultValue(typeof(SmoothingMode), "HighQuality")]
        public SmoothingMode SmoothingMode
        {
            get { return smoothingMode; }
            set
            {
                if (value != SmoothingMode.Invalid)
                {
                    smoothingMode = value; 
                    this.Invalidate();
                }
            }
        }

        #endregion


        /// <summary>
        /// If any errors occur, this will contain the errors information. 
        /// HasErrors will be set to true if any errors have occured.
        /// </summary>
        [Description("If any errors occur, this will contain the errors information. " +
            "HasErrors will be set to true if any errors have occured")]
        public string ErrorLog
        {
            get { return errorLog; }
        }

        /// <summary>
        /// If any errors have occured, this will be set to true.
        /// </summary>
        [Description("If any errors have occured, this will be set to true")]
        public bool HasErrors
        {
            get { return hasErrors; }
        }

        /// <summary>
        /// Clears any errors and sets HasErrors to false.
        /// </summary>
        public void ClearErrors()
        {
            errorLog = "";
            hasErrors = false;
        }

        /// <summary>
        /// Draws the progress bar.
        /// </summary>
        /// <param name="pe">PaintEventArgs</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            //base.OnPaint(e);

            try
            {
                //Don't bother drawing if there's no surface area to work with
                if (this.Width <= 0 || this.Height <= 0)
                {
                    return;
                }

                pe.Graphics.CompositingMode = compositingMode;
                pe.Graphics.CompositingQuality = compositingQuality;
                pe.Graphics.InterpolationMode = interpolationMode;
                pe.Graphics.PixelOffsetMode = pixelOffsetMode;
                pe.Graphics.SmoothingMode = smoothingMode;

                //Draw bar
                if (currentValue != 0)
                {
                    using (SolidBrush brush = new SolidBrush(barColor))
                    {
                        if (orientation == Orientation.Vertical)
                        {
                            int scaledHeight = this.Height - Convert.ToInt32(((double)this.Height / maximum) * currentValue);
                            pe.Graphics.FillRectangle(brush, 0, scaledHeight, this.Width, this.Height);
                        }
                        else
                        {
                            int scaledWidth = Convert.ToInt32(((double)this.Width / maximum) * currentValue);
                            pe.Graphics.FillRectangle(brush, 0, 0, scaledWidth, this.Height);
                        }
                    }
                }

                //Draw text
                if (textStyle != TextStyleType.None)
                {
                    if (textColor != Color.Transparent)
                    {
                        using (Font font = new Font(this.Font.Name, this.Font.SizeInPoints, this.Font.Style, GraphicsUnit.Pixel))
                        {
                            string txt = null;

                            if (textStyle == TextStyleType.Value)
                            {
                                txt = currentValue.ToString();
                            }
                            else if (textStyle == TextStyleType.ValueOverMaximum)
                            {
                                txt = String.Format("{0}/{1}", currentValue, maximum);
                            }
                            else if (textStyle == TextStyleType.Percentage && maximum != 0)
                            {
                                double p = Convert.ToDouble((100d / maximum) * Value);
                                txt = String.Format("{0}%", p);
                            }
                            else if (textStyle == TextStyleType.Text && !String.IsNullOrWhiteSpace(this.Text))
                            {
                                txt = this.Text;
                            }
                            
                            if (txt != null)
                            {
                                TextRenderer.DrawText(pe.Graphics, txt, font, new Rectangle(0, 0, this.Width, this.Height), textColor, flags);
                            }

                        }
                    }
                }

                //Draw border
                if (borderThickness > 0)
                {
                    if (borderColor != Color.Transparent)
                    {
                        using (Pen pen = new Pen(borderColor, borderThickness))
                        {
                            pe.Graphics.DrawRectangle(pen, 0, 0, this.Width, this.Height);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                errorLog += "Error in OnPaint event\n " + 
                    "Message: " + ex.Message + "\n" + 
                    "Type: " + ex.GetType().ToString() + "\n";
                
                hasErrors = true;
            }

        }


    }

    #endregion

    #region ValueChangedEventArgs : EventArgs

    /// <summary>
    /// Event arguments for the ValueChanged ProgressBar event
    /// </summary>
    public class ValueChangedEventArgs : EventArgs
    {
        /// <summary>
        /// ValueChangedEventArgs
        /// </summary>
        /// <param name="currentValue">The current value of the ProgressBar</param>
        public ValueChangedEventArgs(int currentValue)
        {
            this.Value = currentValue;
        }

        /// <summary>
        /// The current value of ProgressBar
        /// </summary>
        public int Value { get; set; }
    }

    #endregion

    public class ProgressBarBasic_Collection : System.Collections.CollectionBase
    {
        public ProgressBarBasic_Collection()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public void Add(ProgressBarBasic m_oProgressBarBasic)
        {
            // vérify if object is not already in
            if (this.List.Contains(m_oProgressBarBasic))
                throw new InvalidOperationException();

            // adding it
            this.List.Add(m_oProgressBarBasic);

            // return collection
            //return this;
        }
        public void Remove(int index)
        {
            // Check to see if there is a widget at the supplied index.
            if (index > Count - 1 || index < 0)
            // If no widget exists, a messagebox is shown and the operation 
            // is canColumned.
            {
                System.Windows.Forms.MessageBox.Show("Index not valid!");
            }
            else
            {
                List.RemoveAt(index);
            }
        }
        public ProgressBarBasic Item(int Index)
        {
            // The appropriate item is retrieved from the List object and
            // explicitly cast to the Widget type, then returned to the 
            // caller.
            return (ProgressBarBasic)List[Index];
        }

    }

}
