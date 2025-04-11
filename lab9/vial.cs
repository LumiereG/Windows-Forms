using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsLab2
{
    public partial class Vial : UserControl
    {
        private int _maxSegments = 5; // Default value
        private List<Color> _segments = new List<Color>();

        [Category("Appearance")]
        [Description("Maximum number of liquid segments the vial can hold.")]
        [DefaultValue(5)]
        public int MaxSegments
        {
            get => _maxSegments;
            set
            {
                if (value > 0)
                {
                    _maxSegments = value;
                    // Optional: Trim segments if new max is smaller than current count
                    if (_segments.Count > _maxSegments)
                    {
                        _segments = _segments.Take(_maxSegments).ToList();
                    }
                    this.Invalidate(); // Redraw control when property changes
                }
            }
        }

        // InitSegmentCount is mainly for design-time initialization or specific setup.
        // The actual data is stored in _segments. We won't use InitSegmentCount directly
        // for runtime logic after initial setup.
        [Category("Appearance")]
        [Description("Initial number of segments (used if Segments collection is empty). Ignored if Segments is set.")]
        [DefaultValue(0)]
        public int InitSegmentCount { get; set; } = 0; // Simpler property, used during setup

        // The Segments property allows direct access but might be complex for designer.
        // We'll manage it internally and provide methods.
        // Make it browsable=false to avoid designer confusion if InitSegmentCount is primary setup.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)] // Hide from designer serialization
        public List<Color> Segments
        {
            get => _segments;
            // Optional: Allow setting externally, but carefully
            // set { _segments = value; this.Invalidate(); }
        }

        // Constructor
        public Vial()
        {
            InitializeComponent();
            // Enable double buffering for smoother drawing
            this.DoubleBuffered = true;
            // Allow this control to receive drops
            this.AllowDrop = true;
        }

        // Helper methods to interact with segments safely
        public bool IsEmpty => _segments.Count == 0;
        public bool IsFull => _segments.Count >= _maxSegments;
        public int CurrentSegments => _segments.Count;

        public void ClearSegments()
        {
            _segments.Clear();
            this.Invalidate();
        }

         public bool AddSegment(Color color)
         {
             if (!IsFull)
             {
                 _segments.Add(color);
                 this.Invalidate();
                 return true;
             }
             return false;
         }

         public Color? PeekTopColor()
         {
             if (!IsEmpty)
             {
                 return _segments[_segments.Count - 1]; // Last item is the top
             }
             return null;
         }

         // Gets the color and count of the contiguous block at the top
         public Tuple<Color, int> GetTopBlockInfo()
         {
             if (IsEmpty) return null;

             Color topColor = PeekTopColor().Value;
             int count = 0;
             for (int i = _segments.Count - 1; i >= 0; i--)
             {
                 if (_segments[i] == topColor)
                 {
                     count++;
                 }
                 else
                 {
                     break; // Stop when color changes
                 }
             }
             return Tuple.Create(topColor, count);
         }

        // Removes the top block of 'count' segments
        public List<Color> RemoveTopBlock(int count)
        {
             if (count <= 0 || count > _segments.Count) return new List<Color>(); // Return empty list if invalid

             int startIndex = _segments.Count - count;
             List<Color> removed = _segments.GetRange(startIndex, count);
             _segments.RemoveRange(startIndex, count);
             this.Invalidate(); // Redraw after removal
             return removed; // Return the removed segments
        }

         // Adds multiple segments (used when pouring)
         public bool AddSegmentBlock(List<Color> block)
        {
             if (CurrentSegments + block.Count <= MaxSegments)
             {
                 _segments.AddRange(block);
                 this.Invalidate();
                 return true;
             }
             return false; // Not enough space
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e); // Call base class method

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // Nicer drawing

            // Define vial appearance parameters (adjust as needed)
            int padding = 5; // Padding around the vial drawing
            int vialWidth = this.ClientSize.Width - 2 * padding;
            int vialHeight = this.ClientSize.Height - 2 * padding;
            float segmentHeight = (float)vialHeight / MaxSegments;

            // Draw vial outline (simple rectangle)
            using (Pen outlinePen = new Pen(Color.Black, 2))
            {
                g.DrawRectangle(outlinePen, padding, padding, vialWidth, vialHeight);
            }

            // Draw segments from bottom to top
            for (int i = 0; i < MaxSegments; i++)
            {
                float yPos = padding + vialHeight - (i + 1) * segmentHeight; // Calculate Y from bottom
                RectangleF segmentRect = new RectangleF(padding, yPos, vialWidth, segmentHeight);

                // Check if this segment index has a color in our list
                if (i < _segments.Count)
                {
                    // Fill with the segment color
                    using (SolidBrush segmentBrush = new SolidBrush(_segments[i]))
                    {
                        g.FillRectangle(segmentBrush, segmentRect);
                    }
                }
                else
                {
                    // Fill empty segments with a background color (optional)
                    // using (SolidBrush emptyBrush = new SolidBrush(Color.LightGray)) // Example
                    // {
                    //     g.FillRectangle(emptyBrush, segmentRect);
                    // }
                    // Or leave it transparent/background color
                }

                // Optionally draw separator lines (subtle)
                using (Pen separatorPen = new Pen(Color.DarkGray, 0.5f))
                {
                     // Don't draw the topmost line inside the vial outline
                     if (i < MaxSegments -1)
                     {
                        g.DrawLine(separatorPen, segmentRect.Left, segmentRect.Top, segmentRect.Right, segmentRect.Top);
                     }
                }
            }
        }

        // --- Drag and Drop Event Handlers ---

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left && !this.IsEmpty)
            {
                // Start the drag operation, passing this control as data
                this.DoDragDrop(this, DragDropEffects.Move);
            }
        }

        protected override void OnDragEnter(DragEventArgs drgevent)
        {
            base.OnDragEnter(drgevent);
            // Check if the dragged data is a VialControl and not itself
            if (drgevent.Data.GetDataPresent(typeof(Vial)))
            {
                Vial sourceVial = drgevent.Data.GetData(typeof(Vial)) as Vial;
                if (sourceVial != null && sourceVial != this)
                {
                    // Allow the move operation
                    drgevent.Effect = DragDropEffects.Move;
                }
                else
                {
                    // Don't allow dropping onto self or if data is wrong type
                    drgevent.Effect = DragDropEffects.None;
                }
            }
            else
            {
                drgevent.Effect = DragDropEffects.None;
            }
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            base.OnDragDrop(drgevent);
            // Get the source control
            if (drgevent.Data.GetDataPresent(typeof(Vial)))
            {
                Vial sourceVial = drgevent.Data.GetData(typeof(Vial)) as Vial;
                Vial targetVial = this; // The current control is the target

                if (sourceVial != null && sourceVial != targetVial)
                {
                    // --- Potion Pouring Logic ---
                    var topBlockInfo = sourceVial.GetTopBlockInfo();
                    if (topBlockInfo == null) return; // Source is empty, nothing to pour

                    Color pourColor = topBlockInfo.Item1;
                    int pourCount = topBlockInfo.Item2;

                    // Check if target can accept the pour
                    bool canPour = false;
                    if (targetVial.IsEmpty)
                    {
                        canPour = true; // Can always pour into an empty vial if space allows
                    }
                    else
                    {
                         // Can pour if top colors match
                         canPour = targetVial.PeekTopColor() == pourColor;
                    }

                    // Check if there is enough space
                    if (canPour && (targetVial.CurrentSegments + pourCount <= targetVial.MaxSegments))
                    {
                         // Perform the pour
                         List<Color> blockToMove = sourceVial.RemoveTopBlock(pourCount);
                         targetVial.AddSegmentBlock(blockToMove);

                         // Optional: Add sound effect here
                         // Optional: Check for win condition after successful pour
                    }
                    else
                    {
                        // Optional: Provide feedback why pour failed (e.g., different color, not enough space)
                        System.Media.SystemSounds.Beep.Play();
                    }
                }
            }
        }
    }
}
