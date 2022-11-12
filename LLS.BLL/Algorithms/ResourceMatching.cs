using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLS.BLL.Algorithms
{
    public class StudentExp
    {
        public int Id;
        public List<int> Resources;
        public Dictionary<int, int> Choices;
        public List<int> ChoicesTried { get; set; } = new List<int>();
        public int? NextChoiceId
        {
            get
            {
                int? choiceId = null;

                foreach (var data in this.Choices)
                {
                    if (!ChoicesTried.Contains(data.Key))
                    {
                        choiceId = data.Key;
                        break;
                    }
                }
                return choiceId;
            }
        }
        public void NextChoiceTried()
        {
            ChoicesTried.Add((int)NextChoiceId);
        }

        public StudentExp(int id, List<int> resources)
        {
            this.Id = id;
            this.Resources = resources;
            this.Choices = new Dictionary<int, int>();
        }
        public int GetRaresrResource(List<MachineExp> machines)
        {
            List<int> resourcenum = new List<int>();
            for (int i = 0; i < this.Resources.Count; i++)
            {
                int SingleRNum = 0; ;
                for (int j = 0; j < machines.Count; j++)
                {
                    if (machines[j].Resources.Contains(this.Resources[i]))
                    {
                        SingleRNum++;
                    }
                }
                resourcenum.Add(SingleRNum);
                SingleRNum = 0;
            }
            return resourcenum.Min();
        }
        public void SetChoices(List<MachineExp> machines)
        {
            int RarestR = this.GetRaresrResource(machines);
            for (int i = 0; i < machines.Count; i++)
            {
                if (this.Resources.All(val => machines[i].Resources.Contains(val)))
                {
                    this.Choices.Add(machines[i].Id, RarestR - machines[i].Resources.Count);
                }
            }
            this.Choices = this.Choices.OrderByDescending(mset => mset.Value).ToDictionary(x => x.Key, x => x.Value);
            
        }

    }

    public class MachineExp
    {
        public int Id;
        public List<int> Resources;
        public Dictionary<int, int> Choices;
        public StudentExp Partner;
        public MachineExp(int id, List<int> resources)
        {
            this.Id = id;
            this.Resources = resources;
            this.Choices = new Dictionary<int, int>();
        }
        public void SetChoices(List<StudentExp> students, List<MachineExp> machines)
        {
            int rarest = 0;
            for (int i = 0; i < students.Count; i++)
            {
                rarest = students[i].GetRaresrResource(machines);
                if (students[i].Resources.All(val => this.Resources.Contains(val)))
                {
                    this.Choices.Add(students[i].Id, rarest * -1);
                }
            }
            this.Choices = this.Choices.OrderByDescending(mset => mset.Value).ToDictionary(x => x.Key, x => x.Value);
            
        }
    }

    public class MatchGame
    {
        public List<StudentExp> Students;
        public List<MachineExp> Machines;
        public MatchGame(List<StudentExp> students, List<MachineExp> machines)
        {
            this.Students = students;
            this.Machines = machines;
            this.init();
        }

        private void init()
        {
            for (int i = 0; i < this.Students.Count; i++)
            {
                this.Students[i].SetChoices(this.Machines);
            }

            for (int i = 0; i < this.Machines.Count; i++)
            {
                this.Machines[i].SetChoices(this.Students, this.Machines);
            }
        }

        public List<MachineExp> GetFinalResult()
        {
            var FreeStudent = new List<StudentExp>(this.Students);
            while (FreeStudent.Count > 0)
            {
                for (int i = 0; i < this.Machines.Count; i++)
                {
                    var SingleMachineChoices = new List<StudentExp>();
                    for (int j = 0; j < FreeStudent.Count; j++)
                    {
                        if (FreeStudent[j].NextChoiceId == this.Machines[i].Id)
                        {
                            SingleMachineChoices.Add(FreeStudent[j]);
                            FreeStudent[j].NextChoiceTried();
                        }
                    }

                    if (SingleMachineChoices.Count > 0)
                    {
                        if (this.Machines[i].Partner != null)
                        {
                            SingleMachineChoices.Add(this.Machines[i].Partner);
                            FreeStudent.Add(this.Machines[i].Partner);
                            this.Machines[i].Partner = null;
                        }
                        this.Machines[i].Partner = SingleMachineChoices[0];
                        for (int k = 0; k < SingleMachineChoices.Count; k++)
                        {
                            var KeepStudent = true;
                            int counter = 0;
                            foreach (var data in this.Machines[i].Choices)
                            {
                                if (data.Key == this.Machines[i].Id)
                                {
                                    KeepStudent = true;
                                    break;
                                }
                                else if (data.Key == SingleMachineChoices[k].Id)
                                {
                                    KeepStudent = false;
                                    break;
                                }
                            }
                            if (!KeepStudent)
                            {
                                this.Machines[i].Partner = SingleMachineChoices[counter];
                            }
                            counter++;
                        }
                        FreeStudent.Remove(this.Machines[i].Partner);

                    }
                }
            }


            return Machines;
            
        }

    }
    
}
