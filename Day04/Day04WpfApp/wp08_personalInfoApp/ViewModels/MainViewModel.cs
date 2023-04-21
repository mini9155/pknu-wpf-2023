using System;

namespace wp08_personalInfoApp.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {

        // 입력 쪽 변수
        private string inFirstName;
        private string inLastName;
        private string inEmail;
        private DateTime inDate;

        //결과 출력쪽 변수
        private string outFirstName;
        private string outLastName;
        private string outEmail;
        private string outDate;  // 생일 날짜 출력할 때 문자열 대체
        private string outAdult;
        private string outBirthDay;
        private string outZodiac;

    }
}
