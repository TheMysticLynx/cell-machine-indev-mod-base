using System.Threading;
using Modding;
using Modding.PublicInterfaces.Cells;

namespace Indev2
{
    public class TroverCellProcessor : SteppedCellProcessor
    {
        public TroverCellProcessor(ICellGrid cellGrid) : base(cellGrid)
        {
        }

        public override string Name => "Trover";
        public override int CellType => 10;
        public override string CellSpriteIndex => "Trover";

        public override bool OnReplaced(BasicCell basicCell, BasicCell replacingCell)
        {
            return true;
        }

        public override bool TryPush(BasicCell cell, Direction direction, int force)
        {
            if(!cell.Frozen)
                if (direction == cell.Transform.Direction)
                    force++;
                else if (direction.Axis == cell.Transform.Direction.Axis)
                    force--;

            if (force <= 0)
                return false;

            var target = cell.Transform.Position + direction.AsVector2Int;
            if (!_cellGrid.InBounds(target))
                return false;
            var targetCell = _cellGrid.GetCell(target);
            if(targetCell != null)
                _cellGrid.RemoveCell(targetCell.Value);

            var useCell = cell;
            if (direction == cell.Transform.Direction && useCell.SpriteVariant == 0)
            {
                useCell.SpriteVariant = 1;
            }

            _cellGrid.MoveCell(useCell, target);
            return true;
        }

        public override void OnCellInit(ref BasicCell cell)
        {

        }

        public override void Step(CancellationToken ct)
        {
            foreach (var cell in GetOrderedCellEnumerable())
            {
                if (ct.IsCancellationRequested)
                    return;
                BasicCell swapCell = cell;
                switch (cell.SpriteVariant)
                {
                    case >0 and <7:
                        swapCell.SpriteVariant++;
                        break;
                    case 7:
                        swapCell.SpriteVariant = 0;
                        break;
                }

                _cellGrid.PushCell(swapCell, swapCell.Transform.Direction, 0);
            }
        }

        public override void Clear()
        {

        }
    }
}