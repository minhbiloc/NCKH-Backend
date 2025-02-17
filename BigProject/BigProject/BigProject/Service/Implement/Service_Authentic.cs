using BigProject.DataContext;
using BigProject.Entities;
using BigProject.Helper;
using BigProject.Payload.Response;
using BigProject.PayLoad.Converter;
using BigProject.PayLoad.DTO;
using BigProject.PayLoad.Request;
using BigProject.Service.Interface;

namespace BigProject.Service.Implement
{
    public class Service_Authentic : IService_Authentic
    {
        private readonly AppDbContext dbContext;

        private readonly ResponseObject<DTO_Register> responseObject;
        private readonly Converter_Register converter_Register;
        private readonly ResponseBase responseBase;

        public Service_Authentic(AppDbContext dbContext, ResponseObject<DTO_Register> responseObject, Converter_Register converter_Register, ResponseBase responseBase)
        {
            this.dbContext = dbContext;
            this.responseObject = responseObject;
            this.converter_Register = converter_Register;
            this.responseBase = responseBase;
        }

        public async Task<ResponseObject<DTO_Register>>  Register(Request_Register request)
        {
            var msv_check  = dbContext.users.FirstOrDefault(x => x.MaSV == request.MaSV);
            if (msv_check != null)
            {
                return responseObject.ResponseObjectError(StatusCodes.Status404NotFound, " Mã Sinh viên  đã tồn tại ", null);
            }
            var name_check = dbContext.users.FirstOrDefault(x => x.Username == request.Username);
            if (name_check != null)
            {
                return  responseObject.ResponseObjectError(StatusCodes.Status404NotFound, "Tài khoản đã tồn tại ", null);
            }
            var email_check = dbContext.users.FirstOrDefault(x => x.Email == request.Email);
            if (email_check != null)
            {
                return responseObject.ResponseObjectError(StatusCodes.Status404NotFound, "Email đã tồn tại", null);
            }
            if (CheckInput.IsPassWord(request.Password) != request.Password)
                return responseObject.ResponseObjectError(StatusCodes.Status400BadRequest, CheckInput.IsPassWord(request.Password), null);
            var phone_check = dbContext.users.FirstOrDefault(x => x.PhoneNumber == request.PhoneNumber);
            if (phone_check != null)
            {
                return responseObject.ResponseObjectError(StatusCodes.Status404NotFound, "số điện thoại đã tồn tại", null);
            }
            var checkEmail = CheckInput.IsValiEmail(request.Email);
            if (!checkEmail)
            {
                return responseObject.ResponseObjectError(StatusCodes.Status400BadRequest, "Email không hợp lệ (thiếu ký tự đặc biệt hoặc sai định dạng)", null);
            }

            string UrlAvt = null;
            var cloudinary = new CloudinaryService();
            if (request.UrlAvatar == null)
            {
                UrlAvt = "https://media.istockphoto.com/id/1300845620/vector/user-icon-flat-isolated-on-white-background-user-symbol-vector-illustration.jpg?s=612x612&w=0&k=20&c=yBeyba0hUkh14_jgv1OKqIH0CCSWU_4ckRkAoy2p73o=";
            }
            else
            {
                if (!CheckInput.IsImage(request.UrlAvatar))
                {
                    return responseObject.ResponseObjectError(StatusCodes.Status400BadRequest, "Định dạng ảnh không hợp lệ !", null);
                }

                UrlAvt = await cloudinary.UploadImage(request.UrlAvatar);
            }

            var register = new User();
            register.MaSV = request.MaSV;
            request.Class = request.Class;
            register.Birthdate = request.Birthdate;
            register.Username = request.Username;
            register.Password = request.Password;
            register.Email = request.Email;
            register.PhoneNumber = request.PhoneNumber;         
            register.UrlAvatar  = UrlAvt;
            register.FullName = request.FullName;
           

            register.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

            Random random = new Random();
            int code = random.Next(100000, 999999);

            EmailTo emailTo = new EmailTo();
            emailTo.Mail = request.Email;
            emailTo.Subject = "MÃ XÁC NHẬN !";
            emailTo.Content = $"Mã xác nhận của bạn là: {code} mã sẽ hết hạn sau 5 phút!";
            emailTo.SendEmailAsync(emailTo);

            register.Email = request.Email;
            register.RoleId = 1;
            dbContext.users.Add(register);
            dbContext.SaveChanges();

            EmailConfirm comfirmEmail = new EmailConfirm();
            comfirmEmail.UserId = register.Id;
            comfirmEmail.Code = $"{code}";
            comfirmEmail.Exprired = DateTime.Now.AddMinutes(5);
            dbContext.emailConfirms.Add(comfirmEmail);
            dbContext.SaveChanges();

            return   responseObject.ResponseObjectSuccess("đăng kí thành công",  converter_Register.EntityToDTO(register));
        }

       
    }
}
