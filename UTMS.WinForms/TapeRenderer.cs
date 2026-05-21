using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using UTMS.Core;

namespace UTMS.WinForms
{
    /// <summary>
    /// Vykresluje pásku simulátoru a spravuje její vizuální posun v panelu formuláře.
    /// </summary>
    internal sealed class TapeRenderer : IDisposable
    {
        private const int CellWidth = 46;
        private const int CellHeight = 46;
        private const int TapeHorizontalPadding = 12;
        private const int EdgeSafetyCells = 1;
        private readonly Panel panel;
        private Bitmap bitmap;
        private double scrollOffsetCells;
        private double dragStartOffsetCells;
        private int dragStartX;
        private bool isDragging;
        private bool viewportWasMovedByUser;
        private bool viewportShouldCenterOnce;
        private bool followsHead;
        private bool disposed;
        private int highlightedCellIndex = -1;
        private char highlightedSymbol;
        private bool hasHighlightedSymbol;

        /// <summary>
        /// Vytvoří renderer napojený na panel pásky.
        /// </summary>
        public TapeRenderer(Panel panel)
        {
            this.panel = panel ?? throw new ArgumentNullException(nameof(panel));
            PropertyInfo doubleBufferedProperty = typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferedProperty?.SetValue(panel, true);
            panel.Cursor = Cursors.Default;
            RecreateBitmap();
        }

        /// <summary>
        /// Aktuální vizuální stav simulace.
        /// </summary>
        public SimulationVisualState VisualState { get; set; }

        /// <summary>
        /// Překreslí bitmapu pásky podle aktuálního simulátoru.
        /// </summary>
        public void Draw(TuringSimulator simulator)
        {
            if (disposed || panel.IsDisposed)
                return;

            RecreateBitmap();
            if (bitmap == null)
                return;

            using (Graphics surface = Graphics.FromImage(bitmap))
            {
                surface.SmoothingMode = SmoothingMode.AntiAlias;
                surface.Clear(Color.FromArgb(246, 248, 250));

                if (simulator == null || simulator.Machine == null)
                {
                    DrawEmptyTape(surface);
                    return;
                }

                TuringMachine machine = simulator.Machine;
                int tapeTop = Math.Max(44, (panel.Height - CellHeight) / 2);
                int tapeWidth = GetTapeViewportWidth();
                double visibleCells = Math.Max(1, (double)tapeWidth / CellWidth);

                UpdateViewport(machine, visibleCells);

                DrawTapeCells(surface, machine, tapeTop, tapeWidth);
                DrawHeadMarker(surface, machine, tapeTop, tapeWidth);
            }
        }

        /// <summary>
        /// Vykreslí připravenou bitmapu na panel.
        /// </summary>
        public void Paint(Graphics graphics, TuringSimulator simulator)
        {
            if (disposed || panel.IsDisposed)
                return;

            if (bitmap == null || bitmap.Width != panel.Width || bitmap.Height != panel.Height)
                Draw(simulator);

            if (bitmap != null)
                graphics.DrawImage(bitmap, new PointF(0, 0));
        }

        /// <summary>
        /// Zahájí ruční posun pohledu na pásku.
        /// </summary>
        public void BeginDrag(MouseEventArgs e, TuringSimulator simulator)
        {
            if (e.Button != MouseButtons.Left || simulator == null || simulator.Machine == null)
                return;

            isDragging = true;
            dragStartX = e.X;
            dragStartOffsetCells = scrollOffsetCells;
            panel.Capture = true;
            panel.Cursor = Cursors.SizeWE;
        }

        /// <summary>
        /// Během tažení přepočítá posun pásky a překreslí panel.
        /// </summary>
        public void Drag(MouseEventArgs e, TuringSimulator simulator)
        {
            if (!isDragging || simulator == null || simulator.Machine == null)
                return;

            double deltaCells = (double)(dragStartX - e.X) / CellWidth;
            scrollOffsetCells = ClampOffset(simulator.Machine, dragStartOffsetCells + deltaCells);
            viewportWasMovedByUser = true;
            Draw(simulator);
            panel.Invalidate();
        }

        /// <summary>
        /// Ukončí ruční tažení pásky.
        /// </summary>
        public void StopDragging()
        {
            isDragging = false;
            panel.Capture = false;
            panel.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Vrátí kurzor panelu, pokud myš opustí pásku mimo aktivní tažení.
        /// </summary>
        public void HandleMouseLeave()
        {
            if (!isDragging)
                panel.Cursor = Cursors.Default;
        }

        /// <summary>
        /// Připraví bitmapu pro novou velikost panelu a překreslí pásku.
        /// </summary>
        public void Resize(TuringSimulator simulator)
        {
            if (disposed || panel.IsDisposed)
                return;

            RecreateBitmap();
            Draw(simulator);
            panel.Invalidate();
        }

        /// <summary>
        /// Vrátí pohled pásky do výchozího vycentrovaného stavu.
        /// </summary>
        public void ResetViewport()
        {
            scrollOffsetCells = 0;
            viewportWasMovedByUser = false;
            viewportShouldCenterOnce = true;
            highlightedCellIndex = -1;
            hasHighlightedSymbol = false;
            StopDragging();
        }

        /// <summary>
        /// Nastaví režim pohledu na pásku.
        /// </summary>
        public void ApplyViewMode(bool followsHead, TuringSimulator simulator)
        {
            this.followsHead = followsHead;
            viewportWasMovedByUser = false;
            viewportShouldCenterOnce = true;
            Draw(simulator);
            panel.Invalidate();
        }

        /// <summary>
        /// Krátce zvýrazní buňku, do které poslední krok zapisoval.
        /// </summary>
        public void HighlightWriteCell(TuringSimulator simulator, int cellIndex, char displayedSymbol)
        {
            if (simulator == null || simulator.Machine == null)
                return;

            highlightedCellIndex = cellIndex;
            highlightedSymbol = displayedSymbol;
            hasHighlightedSymbol = true;
            Draw(simulator);
            panel.Invalidate();
        }

        /// <summary>
        /// Odstraní dočasné zvýraznění zápisu.
        /// </summary>
        public void ClearWriteHighlight()
        {
            highlightedCellIndex = -1;
            hasHighlightedSymbol = false;
        }

        /// <summary>
        /// Uvolní bitmapu rendereru.
        /// </summary>
        public void Dispose()
        {
            disposed = true;
            bitmap?.Dispose();
            bitmap = null;
        }

        /// <summary>
        /// Vykreslí prázdný stav panelu před načtením programu.
        /// </summary>
        private void DrawEmptyTape(Graphics surface)
        {
            using (Font font = new Font("Segoe UI", 10, FontStyle.Regular))
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(90, 98, 110)))
            {
                Rectangle bounds = new Rectangle(0, 0, panel.Width, panel.Height);
                using (StringFormat format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    surface.DrawString("No machine loaded", font, textBrush, bounds, format);
                }
            }
        }

        /// <summary>
        /// Vykreslí viditelnou část buněk pásky podle aktuálního posunu.
        /// </summary>
        private void DrawTapeCells(Graphics surface, TuringMachine machine, int tapeTop, int tapeWidth)
        {
            int firstCell = Math.Max(0, (int)Math.Floor(scrollOffsetCells) - 1);
            int lastCell = Math.Min(machine.Cells.Count - 1, (int)Math.Ceiling(scrollOffsetCells + (double)tapeWidth / CellWidth) + 1);
            Rectangle viewportBounds = new Rectangle(TapeHorizontalPadding, 0, tapeWidth, panel.Height);
            GraphicsState clipState = surface.Save();

            try
            {
                surface.SetClip(viewportBounds);

                using (Font symbolFont = new Font("Consolas", 18, FontStyle.Bold))
                using (Font indexFont = new Font("Segoe UI", 7, FontStyle.Regular))
                using (Pen borderPen = new Pen(Color.FromArgb(208, 215, 222)))
                using (Pen headPen = new Pen(Color.FromArgb(9, 105, 218), 2))
                using (SolidBrush cellBrush = new SolidBrush(Color.White))
                using (SolidBrush blankBrush = new SolidBrush(Color.FromArgb(246, 248, 250)))
                using (SolidBrush activeBrush = new SolidBrush(Color.FromArgb(221, 244, 255)))
                using (SolidBrush writeBrush = new SolidBrush(Color.FromArgb(255, 248, 197)))
                using (SolidBrush symbolBrush = new SolidBrush(Color.FromArgb(31, 35, 40)))
                using (SolidBrush blankTextBrush = new SolidBrush(Color.FromArgb(140, 149, 159)))
                using (SolidBrush indexBrush = new SolidBrush(Color.FromArgb(110, 118, 129)))
                using (StringFormat centered = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                {
                    Rectangle headBounds = Rectangle.Empty;
                    bool hasHeadBounds = false;

                    for (int index = firstCell; index <= lastCell; index++)
                    {
                        int x = TapeHorizontalPadding + (int)Math.Round((index - scrollOffsetCells) * CellWidth);
                        Rectangle cellBounds = new Rectangle(x, tapeTop, CellWidth, CellHeight);
                        bool isHead = index == machine.HeadIndex();
                        bool isBlank = machine.Cells[index] == machine.BlankSymbol;
                        bool isHighlightedWrite = index == highlightedCellIndex;
                        char symbol = isHighlightedWrite && hasHighlightedSymbol ? highlightedSymbol : machine.Cells[index];

                        surface.FillRectangle(isHighlightedWrite ? writeBrush : isHead ? activeBrush : isBlank ? blankBrush : cellBrush, cellBounds);
                        surface.DrawRectangle(borderPen, cellBounds);
                        surface.DrawString(symbol.ToString(), symbolFont, isBlank && !isHighlightedWrite ? blankTextBrush : symbolBrush, cellBounds, centered);
                        surface.DrawString(index.ToString(), indexFont, indexBrush, new Rectangle(x, tapeTop + CellHeight + 4, CellWidth, 14), centered);

                        if (isHead)
                        {
                            headBounds = cellBounds;
                            hasHeadBounds = true;
                        }
                    }

                    if (hasHeadBounds)
                    {
                        Rectangle headBorderBounds = new Rectangle(headBounds.Left, headBounds.Top, headBounds.Width - 1, headBounds.Height - 1);
                        surface.DrawRectangle(headPen, headBorderBounds);
                    }
                }
            }
            finally
            {
                surface.Restore(clipState);
            }
        }

        /// <summary>
        /// Zvýrazní pozici hlavy nad buňkou pásky.
        /// </summary>
        private void DrawHeadMarker(Graphics surface, TuringMachine machine, int tapeTop, int tapeWidth)
        {
            int headX = TapeHorizontalPadding + (int)Math.Round((machine.HeadIndex() - scrollOffsetCells) * CellWidth);
            int tapeRight = TapeHorizontalPadding + tapeWidth;
            if (headX + CellWidth < TapeHorizontalPadding || headX > tapeRight)
                return;

            Rectangle viewportBounds = new Rectangle(TapeHorizontalPadding, 0, tapeWidth, panel.Height);
            GraphicsState clipState = surface.Save();

            try
            {
                surface.SetClip(viewportBounds);

                using (Font stateFont = new Font("Segoe UI", 9, FontStyle.Bold))
                using (SolidBrush markerBrush = new SolidBrush(Color.FromArgb(9, 105, 218)))
                using (SolidBrush textBrush = new SolidBrush(Color.White))
                using (StringFormat centered = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter })
                {
                    Point[] triangle =
                    {
                        new Point(headX + CellWidth / 2, tapeTop - 8),
                        new Point(headX + CellWidth / 2 - 7, tapeTop - 20),
                        new Point(headX + CellWidth / 2 + 7, tapeTop - 20)
                    };
                    surface.FillPolygon(markerBrush, triangle);

                    Rectangle stateBounds = new Rectangle(headX - 20, tapeTop + CellHeight + 24, CellWidth + 40, 24);
                    surface.FillRectangle(markerBrush, stateBounds);
                    surface.DrawString(machine.CurrentState(), stateFont, textBrush, stateBounds, centered);
                }
            }
            finally
            {
                surface.Restore(clipState);
            }
        }

        /// <summary>
        /// Připraví prázdnou bitmapu pro aktuální velikost panelu.
        /// </summary>
        private void RecreateBitmap()
        {
            if (disposed || panel.IsDisposed)
                return;

            int width = Math.Max(1, panel.Width);
            int height = Math.Max(1, panel.Height);
            if (bitmap != null && bitmap.Width == width && bitmap.Height == height)
                return;

            bitmap?.Dispose();
            bitmap = new Bitmap(width, height);
        }

        /// <summary>
        /// Omezí posun pásky na existující rozsah buněk.
        /// </summary>
        private double ClampOffset(TuringMachine machine, double offset)
        {
            int tapeWidth = GetTapeViewportWidth();
            double visibleCells = Math.Max(1, (double)tapeWidth / CellWidth);
            double maxOffset = Math.Max(0, machine.Cells.Count - visibleCells);

            if (offset < 0)
                return 0;
            if (offset > maxOffset)
                return maxOffset;

            return offset;
        }

        /// <summary>
        /// Vrátí šířku vnitřní oblasti, do které se kreslí buňky pásky.
        /// </summary>
        private int GetTapeViewportWidth()
        {
            return Math.Max(CellWidth, panel.Width - 2 * TapeHorizontalPadding);
        }

        /// <summary>
        /// Upraví posun pásky podle zvoleného režimu pohledu.
        /// </summary>
        private void UpdateViewport(TuringMachine machine, double visibleCells)
        {
            if (isDragging)
            {
                scrollOffsetCells = ClampOffset(machine, scrollOffsetCells);
                return;
            }

            if (!viewportWasMovedByUser && followsHead)
            {
                scrollOffsetCells = CenterOnHead(machine, visibleCells);
                viewportShouldCenterOnce = false;
                return;
            }

            if (viewportShouldCenterOnce)
            {
                scrollOffsetCells = CenterOnHead(machine, visibleCells);
                viewportShouldCenterOnce = false;
                return;
            }

            if (!followsHead)
            {
                if (VisualState != SimulationVisualState.Running)
                {
                    scrollOffsetCells = ClampOffset(machine, scrollOffsetCells);
                    return;
                }

                scrollOffsetCells = KeepHeadInSafeZone(machine, visibleCells);
                return;
            }

            scrollOffsetCells = ClampOffset(machine, scrollOffsetCells);
        }

        /// <summary>
        /// Vrátí posun pásky tak, aby byla hlava přibližně uprostřed viditelné oblasti.
        /// </summary>
        private double CenterOnHead(TuringMachine machine, double visibleCells)
        {
            return ClampOffset(machine, machine.HeadIndex() - visibleCells / 2 + 0.5);
        }

        /// <summary>
        /// V režimu pohyblivé hlavy posune pásku jen tehdy, když se hlava blíží k okraji.
        /// </summary>
        private double KeepHeadInSafeZone(TuringMachine machine, double visibleCells)
        {
            double offset = ClampOffset(machine, scrollOffsetCells);
            double safetyCells = Math.Min(EdgeSafetyCells, Math.Max(0, (visibleCells - 1) / 2));
            double safeLeft = offset + safetyCells;
            double safeRight = offset + visibleCells - safetyCells - 1;
            int headIndex = machine.HeadIndex();

            if (headIndex < safeLeft)
                return ClampOffset(machine, headIndex - safetyCells);

            if (headIndex > safeRight)
                return ClampOffset(machine, headIndex - visibleCells + safetyCells + 1);

            return offset;
        }
    }
}
