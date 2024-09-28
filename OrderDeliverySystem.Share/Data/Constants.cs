namespace OrderDeliverySystem.Share.Data
{
    public class Constants
    {
        public class User
        {
            public const int MinNameLength = 1;
            public const int MaxNameLength = 50;
            public const int MinEmailLength = 3;
            public const int MaxEmailLength = 50;
            public const int MinPasswordLength = 6;
            public const int MaxPasswordLength = 100;
            public const int PhoneNumberLength = 12;
            public const string PasswordRegularExpression = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$";
            public const string PhoneNumberRegularExpression = @"^\d{3}-\d{3}-\d{4}$";
        }

        public class Address
        {
            public const int MaxUnitLength = 20;
            public const int MaxAddressLength = 255;
            public const int MaxCityLength = 100;
            public const int MaxProvinceLength = 2;
            public const int MaxPostalCodeLength = 7;
        }

        public class Merchant
        {
            public const int MaxBusinessNameLength = 20;
            public const int MaxPicLength = 255;
            public const string DefaultPic = "https://www.eclosio.ong/wp-content/uploads/2018/08/default.png";
        }
    }
}
