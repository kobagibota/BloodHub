namespace BloodHub.Client.Helpers
{
    public static class AppConstants
    {
        // Key lưu trữ AccessToken và RefreshToken trong LocalStorage
        public const string AccessTokenKey = "AccessToken";
        public const string RefreshTokenKey = "RefreshToken";

        // Endpoint Auth
        public static class AuthEndpoints
        {
            public const string Login = "api/auth/login";
            public const string RefreshToken = "api/auth/refresh";
            public const string CurrentUser = "api/auth/me";
            public const string ResetPassword = "api/auth/reset-password";
            public const string ChangePassword = "api/auth/change-password";
        }

        // Endpoint Doctor
        public static class DoctorEndpoints
        {
            public const string GetAll = "api/doctor";
            public const string GetById = "api/doctor/getbyid";
            public const string Create = "api/doctor/create";
            public const string Update = "api/doctor/update";
            public const string Delete = "api/doctor/delete";
        }

        // Endpoint Doctor
        public static class NursingEndpoints
        {
            public const string GetAll = "api/nursing";
            public const string GetById = "api/nursing/getbyid";
            public const string Create = "api/nursing/create";
            public const string Update = "api/nursing/update";
            public const string Delete = "api/nursing/delete";
        }

        // Endpoint Ward
        public static class WardEndpoints
        {
            public const string GetAll = "api/ward";
            public const string GetById = "api/ward/getbyid";
            public const string Create = "api/ward/create";
            public const string Update = "api/ward/update";
            public const string Delete = "api/ward/delete";
        }

        // Endpoint Ward
        public static class ProductEndpoints
        {
            public const string GetAll = "api/product";
            public const string GetById = "api/product/getbyid";
            public const string Create = "api/product/create";
            public const string Update = "api/product/update";
            public const string Delete = "api/product/delete";
        }

        // Endpoint Patient
        public static class PatientEndpoints
        {
            public const string GetAll = "api/patient";
            public const string GetById = "api/patient/getbyid";
            public const string Create = "api/patient/create";
            public const string Update = "api/patient/update";
            public const string Delete = "api/patient/delete";
        }

        // Endpoint Order
        public static class OrderEndpoints
        {
            public const string GetAll = "api/order";
            public const string GetById = "api/order/getbyid";
            public const string GetByPatientId = "api/order/getbypatientid";
            public const string Create = "api/order/create";
            public const string Update = "api/order/update";
            public const string Delete = "api/order/delete";
        }
    }
}
