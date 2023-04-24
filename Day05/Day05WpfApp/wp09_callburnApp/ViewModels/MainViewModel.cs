using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wp09_callburnApp.Models;

namespace wp09_callburnApp.ViewModels
{
    public class MainViewModel : Screen
    {
        private string firstName;
        public string FirstName {
            get => firstName;
            set
            {
                firstName = value;
                NotifyOfPropertyChange(() => FirstName);
                NotifyOfPropertyChange(nameof(CanClearName));
                NotifyOfPropertyChange(nameof(FirstName));
            }
        }

        private string lastName;

        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                NotifyOfPropertyChange(() => LastName);
                NotifyOfPropertyChange(nameof(CanClearName));
                NotifyOfPropertyChange(() => FullName);
            }
        }
        public string FullName
        {
            get => $"{LastName} {FirstName}";
        }

        private BindableCollection<Person> managers = new BindableCollection<Person>();

        public BindableCollection<Person> Managers
        {
            get => managers; 
            set => managers = value;
        }

        //콤보박스에 선택된 값을 지정할 속성
        private Person selectedManager;

        public Person SelectedManager
        {
            get => selectedManager;
            set
            {
                selectedManager = value;
                LastName = selectedManager.LastName;
                FirstName = selectedManager.FirstName;
                NotifyOfPropertyChange(nameof(SelectedManager));
            }
        }

        public MainViewModel()
        {
            Managers.Add(new Person { FirstName = "Tim", LastName = "Carmack" });
            Managers.Add(new Person { FirstName = "stive", LastName = "Jobs" });
            Managers.Add(new Person { FirstName = "Bill", LastName = "gate" });
            Managers.Add(new Person { FirstName = "Tom", LastName = "cruse" });
        }

        public void ClearName()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
        }

        public bool CanClearName
        {
            get => !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName); // 둘 다 안 비어 있으면
        }
    }
}
