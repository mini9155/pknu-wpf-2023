using System;
using wp08_personalInfoApp.Logics;

namespace wp08_personalInfoApp.Models
{
    internal class Person
    {
        private string email;
        private DateTime date;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email
        {
            get => email;
            set
            {
                if (Commons.IsValidEmail(value) != true)
                {
                    throw new Exception("유효하지 않은 이메일 형식");
                }
                else
                {
                    email = value;
                }
            }
        }
        public DateTime Date
        {
            get => date;
            set
            {
                var result = Commons.GetAge(value);
                if (result > 120 || result <= 0)
                {
                    throw new Exception("유효하지 않은 생일");
                }
                else
                {
                    date = value;
                }
            }
        }

        public bool IsAdult
        {
            get
            {
                return Commons.GetAge(Date) > 18;
            }
        }

        public bool IsBirthDay
        {
            get
            {
                return DateTime.Now.Month == Date.Month && DateTime.Now.Day == Date.Day;
            }
        }

        public string Zodiac
        {
            get => Commons.GetZodiac(Date);
        }
        public Person(string firstname, string lastname, string email, DateTime date)
        {
            FirstName = firstname;
            LastName = lastname;
            Email = email;
            Date = date;
        }
    }
}
