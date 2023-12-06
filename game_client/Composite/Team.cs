using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Composite
{
    public class Team : IGameComponent
    {
        private List<IGameComponent> _members = new List<IGameComponent>();
        private string _teamName;
        private bool _increaseSize;

        public Team(string teamName, bool increaseSize)
        {
            _teamName = teamName;
            _increaseSize = increaseSize;
        }

        public void Add(IGameComponent member) => _members.Add(member);
        public void Remove(IGameComponent member) => _members.Remove(member);
        public IGameComponent GetChild(int index) => _members[index];
        public void Operation()
        {
            Console.WriteLine($"Team {_teamName} is performing a group operation.");
            foreach (var member in _members)
            {
                member.Operation();
                if (_increaseSize) member.IncreaseSize(); 
                else member.ShapeShift();
            }
        }

        public bool IsComposite() => true;

        public void IncreaseSize() {
            foreach (var member in _members)
            {
                member.IncreaseSize();
            }
        }
        public void ShapeShift() {
            foreach (var member in _members)
            {
                member.ShapeShift();
            }
        }
    }
}
