using Alophia.Services;
using Alophia.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI;

namespace Alophia.Views
{
    public sealed partial class StagesPage : Page
    {
        private MainViewModel ViewModel { get; set; }
        private ISimulationService SimulationService { get; set; }

        // Layout constants
        private const float LabelColumnWidth = 180;
        private const float LeftMargin = 200;
        private const float TopMargin = 60;
        private const float RowHeight = 50;
        private const float StageBarHeight = 24;
        private const float PixelsPerWeek = 80;

        public StagesPage()
        {
            InitializeComponent();
            ViewModel = App.ServiceProvider.GetRequiredService<MainViewModel>();
            SimulationService = App.ServiceProvider.GetRequiredService<ISimulationService>();

            // Hook into simulation ticks to invalidate and redraw
            if (SimulationService is SimulationService simService)
            {
                simService.Tick += (s, e) => GanttChart.Invalidate();
            }
        }

        private void GanttChart_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var session = args.DrawingSession;
            var project = ViewModel.Project;

            if (project?.Stages == null || project.Stages.Count == 0)
            {
                session.DrawText("No project loaded", LeftMargin, TopMargin, Colors.Gray);
                return;
            }

            // Draw week grid and labels
            DrawWeekGrid(session, sender.ActualWidth);

            // Draw stages
            float yPos = TopMargin;
            for (int i = 0; i < project.Stages.Count; i++)
            {
                var stage = project.Stages[i];
                DrawStageRow(session, stage, yPos);
                yPos += RowHeight;
            }
        }

        private void DrawWeekGrid(CanvasDrawingSession session, double canvasWidth)
        {
            const int maxWeeks = 20;
            var gridColor = Color.FromArgb(25, 128, 128, 128); // Subtle gray

            // Draw week labels and grid lines
            for (int week = 0; week <= maxWeeks; week++)
            {
                float x = LeftMargin + (week * PixelsPerWeek);
                if (x > canvasWidth) break;

                // Vertical grid line
                session.DrawLine(x, TopMargin - 30, x, (float)canvasWidth, gridColor, 1);

                // Week label
                string label = $"W{week}";
                session.DrawText(label, x - 15, TopMargin - 50, Colors.Gray,
                    new CanvasTextFormat { FontSize = 11 });
            }
        }

        private void DrawStageRow(CanvasDrawingSession session, Models.Stage stage, float yPos)
        {
            // Parse stage colors
            var (colorSolid, colorLight) = ParseColor(stage.ColorSolid, stage.ColorLight);

            // Draw stage label in left column
            var labelFormat = new CanvasTextFormat
            {
                FontSize = 12,
                HorizontalAlignment = CanvasHorizontalAlignment.Left
            };
            session.DrawText(stage.Label, 12, yPos + 13, Colors.White, labelFormat);

            // Draw planned duration bar (light color)
            float barY = yPos + (RowHeight - StageBarHeight) / 2;
            float startX = LeftMargin + (stage.Start * PixelsPerWeek / 5); // 5 ticks per week
            float plannedWidth = (stage.Duration * PixelsPerWeek / 5);

            // Background for bar
            session.FillRectangle(startX, barY, plannedWidth, StageBarHeight, colorLight);
            session.DrawRectangle(startX, barY, plannedWidth, StageBarHeight, colorSolid, 1);

            // Draw elapsed bar (solid color) based on current ticks
            int elapsedTicks = ViewModel.TicksElapsed;
            if (elapsedTicks > stage.Start)
            {
                int ticksIntoStage = Math.Min(elapsedTicks - stage.Start, stage.Duration);
                float elapsedWidth = (ticksIntoStage * PixelsPerWeek / 5);
                session.FillRectangle(startX, barY, elapsedWidth, StageBarHeight, colorSolid);
            }
        }

        private (Color solid, Color light) ParseColor(string hexSolid, string hexLight)
        {
            try
            {
                var solid = ParseHexColor(hexSolid);
                var light = ParseHexColor(hexLight);
                return (solid, light);
            }
            catch
            {
                // Fallback to default purple
                return (Color.FromArgb(255, 83, 74, 183), Color.FromArgb(255, 238, 237, 254));
            }
        }

        private Color ParseHexColor(string hex)
        {
            hex = hex.TrimStart('#');
            if (hex.Length == 6)
            {
                int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                return Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
            }
            return Colors.Gray;
        }
    }
}
