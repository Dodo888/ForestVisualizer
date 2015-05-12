using System;
using ForestSolver;
using ForestSolverPackages;

namespace Client
{
    class ClientWorker
    {
        private KeeperAI ai;
        private ForestKeeper keeper;

        public void Initialise(ClientInfo info, string name)
        {
            keeper = new ForestKeeper(name, ForestSolver.Point.ConvertFromNetPoint(info.StartPosition).Reverse(), info.Hp, 1);
            ai = new KeeperAI(keeper, ForestSolver.Point.ConvertFromNetPoint(info.Target).Reverse());
        }

        public void Move(Func<ForestKeeper, DeltaPoint, bool> tryMove)
        {
            ai.Go(tryMove);
        }

        public void ChangePosition(DeltaPoint point)
        {
            keeper.position = keeper.position.Add(point);
        }
    }
}
