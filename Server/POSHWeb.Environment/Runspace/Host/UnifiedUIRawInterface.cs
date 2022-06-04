using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Host;
using System.Text;
using System.Threading.Tasks;

namespace POSHWeb.Environment.Runspace.Host
{
    public class UnifiedUIRawInterface : PSHostRawUserInterface
    {
        public override ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
        public override Size BufferSize { get; set; } = new(300, 5000);
        public override Coordinates CursorPosition { get; set; }
        public override int CursorSize { get; set; }
        public override ConsoleColor ForegroundColor { get; set; }

        public override bool KeyAvailable => false;

        public override Size MaxPhysicalWindowSize => new(300, 5000);

        public override Size MaxWindowSize => new(300, 5000);

        public override Coordinates WindowPosition { get; set; }
        public override Size WindowSize { get; set; }

        public override string WindowTitle { get; set; }

        public override void FlushInputBuffer()
        {
        }

        public override BufferCell[,] GetBufferContents(Rectangle rectangle) => null;

        public override KeyInfo ReadKey(ReadKeyOptions options) => default;

        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip,
            BufferCell fill)
        {
        }
        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
        {
        }
        public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
        {
        }
    }
}