using Domain.Shared.Settings;
using System.Net;

namespace Domain.Shared.Configurations
{
    public static class EmailTemplate
    {
        public static string WelcomeEmailTemplate(string userName, string loginId, string password, string? loginUrl)
        {
            return string.Format(@"
            <table role='presentation' cellpadding='0' cellspacing='0' width='100%'
                   style='background:#ffffff;font-family:Segoe UI,Helvetica,Arial,sans-serif;'>
              <tr>
                <td align='center' style='padding:40px 12px;'>
                  <table role='presentation' cellpadding='0' cellspacing='0' width='600'
                         style='border:1px solid #d2d6dc;border-radius:8px;
                                box-shadow:0 2px 8px rgba(0,0,0,0.05);'>

                    <!-- Header -->
                    <tr>
                      <td style='background:#002b55;padding:20px;
                                 border-top-left-radius:8px;border-top-right-radius:8px;'>
                        <h2 style='margin:0;font-size:20px;font-weight:600;
                                   color:#ffffff;text-align:center;'>
                          Welcome to NGIT Services
                        </h2>
                      </td>
                    </tr>

                    <!-- Body -->
                    <tr>
                      <td style='padding:30px 25px;line-height:1.6;color:#333333;'>
                        <p style='font-size:15px;margin-top:0;'>Dear User,</p>

                        <p style='font-size:15px;'>
                          Your account has been successfully created.  
                          Please use the following credentials to log in for the first time:
                        </p>

                        <table cellpadding='6' cellspacing='0' style='margin:15px 0;'>
                          <tr>
                            <td><strong>Login ID:</strong></td>
                            <td>{1}</td>
                          </tr>
                          <tr>
                            <td><strong>Password:</strong></td>
                            <td>{2}</td>
                          </tr>
                        </table>

                        <p style='font-size:14px;color:#555;'>
                          For security reasons, you will be required to change your password
                          immediately after your first login.
                        </p>
                      
                        <p style='font-size:13px;color:#555;'>
                          If you did not request this account, please contact support immediately.
                        </p>
                      </td>
                    </tr>

                    <!-- Footer -->
                    <tr>
                      <td style='background:#f1f3f5;padding:20px;
                                 border-bottom-left-radius:8px;border-bottom-right-radius:8px;
                                 font-size:12px;color:#6b7280;text-align:center;'>
                        This is an automated message. Please do not reply.<br/>
                        © NGIT Services
                      </td>
                    </tr>

                  </table>
                </td>
              </tr>
            </table>",
            WebUtility.HtmlEncode(userName),
            WebUtility.HtmlEncode(loginId),
            WebUtility.HtmlEncode(password));
        }


        public static string ForgotPasswordOTP(string otp)
        {
            return string.Format(@"
                <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='background:#ffffff;
                       font-family:Segoe UI,Helvetica,Arial,sans-serif;'>
                  <tr>
                    <td align='center' style='padding:40px 12px;'>
                      <table role='presentation' cellpadding='0' cellspacing='0' width='600'
                             style='border:1px solid #d2d6dc;border-radius:8px;box-shadow:0 2px 8px rgba(0,0,0,0.05);'>

                        <!-- Header -->
                        <tr>
                          <td style='background:#002b55;padding:20px;border-top-left-radius:8px;border-top-right-radius:8px;'>
                            <h2 style='margin:0;font-size:20px;font-weight:600;color:#ffffff;text-align:center;'>
                              Verify Your Account
                            </h2>
                          </td>
                        </tr>

                        <!-- Body -->
                        <tr>
                          <td style='padding:30px 25px;line-height:1.6;color:#333333;'>
                            <p style='font-size:15px; color: #333333; margin-top:0;'>Dear User,</p>
                            <p style='font-size:15px; color: #333333;'>
                              We have received a request to reset your password. Please use the following One-Time Password (OTP)
                              to verify your identity. <strong>This OTP is valid for 3 minutes</strong>.
                            </p>

                            <div style='text-align: center; margin: 30px 0;'>
                              <span style='display: inline-block; font-size: 24px; background-color: #002b55; color: #ffffff;
                                           padding: 12px 24px; border-radius: 4px; font-weight: bold;
                                           white-space: nowrap; word-break: normal;'>
                                {0}
                              </span>
                            </div>

                            <p style='font-size:14px; color:#555555;'>
                              If you did not request this password reset, you may safely ignore this email. No further action is required.
                            </p>
                          </td>
                        </tr>

                        <!-- Footer -->
                        <tr>
                          <td style='background:#f1f3f5;padding:20px;border-bottom-left-radius:8px;border-bottom-right-radius:8px;
                                     font-size:12px;color:#6b7280;text-align:center;'>
                            This is an automated message. Please do not reply.<br/>
                            © NGIT Services
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>",
            WebUtility.HtmlEncode(otp.Trim()));
        }

        public static string SendDefaultPasswordForgotPassword(string userName, string tempPassword)
        {
            return string.Format(@"
                <table role='presentation' cellpadding='0' cellspacing='0' width='100%' style='background:#ffffff;
                       font-family:Segoe UI,Helvetica,Arial,sans-serif;'>
                  <tr>
                    <td align='center' style='padding:40px 12px;'>
                      <table role='presentation' cellpadding='0' cellspacing='0' width='620'
                             style='border:1px solid #d2d6dc;border-radius:8px;box-shadow:0 2px 8px rgba(0,0,0,0.05);'>
                        <!-- Header bar -->
                        <tr>
                          <td style='background:#17375e;padding:20px;border-top-left-radius:8px;border-top-right-radius:8px;'>
                            <h2 style='margin:0;font-size:20px;font-weight:600;color:#ffffff;text-align:center;'>
                              Password Reset Details
                            </h2>
                          </td>
                        </tr>
                        <!-- Body -->
                        <tr>                        
                           <td style='padding:30px 25px;line-height:1.6;color:#333333;'>
                            <p style='font-size:16px; color: #333333; margin-top:0;'>Dear User,</p>
                            <p style='font-size:16px; color: #333333;'>
                                A temporary password has been generated for your account. Please find the details below:
                            </p>
                            <!-- Credentials table -->
                            <table role='presentation' cellpadding='0' cellspacing='0' width='100%'
                                   style='border:1px solid #e5e7eb;border-collapse:collapse;'>
                              <tr style='background:#f8f9fa;'>
                                <th align='left' style='padding:12px 16px;font-size:14px;color:#555;font-weight:600;
                                                        border-bottom:1px solid #e5e7eb;width:180px;'>
                                  Username
                                </th>
                                <td style='padding:12px 16px;font-size:14px;color:#111111;border-bottom:1px solid #e5e7eb;
                                           word-break:normal; white-space:nowrap; overflow-wrap:break-word;'>
                                  {0}
                                </td>
                              </tr>
                              <tr>
                                <th align='left' style='padding:12px 16px;font-size:14px;color:#555;font-weight:600;
                                                        background:#f8f9fa;width:180px;'>
                                  Password
                                </th>
                                <td style='padding:12px 16px;font-size:16px;font-weight:700;color:#17375e;
                                           word-break:normal; white-space:nowrap; overflow-wrap:break-word;'>
                                  {1}
                                </td>
                              </tr>
                            </table>

                            <p style='margin:24px 0 0;font-size:14px;color:#444444;'>
                              <strong>Important: Please sign in and change this password immediately</strong>.
                              Do not share your login details with anyone.
                            </p>
                          </td>
                        </tr>
                        <!-- Footer -->
                        <tr>
                          <td style='background:#f1f3f5;padding:20px;border-bottom-left-radius:8px;border-bottom-right-radius:8px;
                                     font-size:12px;color:#6b7280;text-align:center;'>
                            This is an automated message. Please do not reply.<br/>
                            © NGIT Services
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>",
                WebUtility.HtmlEncode(userName.Trim()),
                WebUtility.HtmlEncode(tempPassword.Trim()));
        }

        public static string Request(string empDtls, string leaveTypeName)
        {

            var template = string.Format(@"<p style='font-family: Century Gothic, serif; font-size: 14px;'>Dear Mr/Ms Recipient,</p>
            <p style='font-family: Century Gothic, serif; font-size: 14px;'><b>{0}</b> has requested <b>{1}</b>.</p>
            <p style='font-family: Century Gothic, serif; font-size: 14px;'>You can track its status through our ERP system by clicking <a href='{3}'>here</a>.</p>
            <br/>
            <p style='font-family: Century Gothic, serif; font-size: 13px;'>Best Regrads</p>
            <p style='font-family: Century Gothic, serif; font-size: 13px;'><b>ProMatrix</b></p>
            <p style='font-family: Century Gothic, serif; font-size: 10px;color:#FF0000'>This is an auto-generated mail and should not be replied to.</p>", empDtls, leaveTypeName, AppSettings.ClientOrigin);
            return template;
        }
        public static string Modified(string empDtls, string leaveTypeName)
        {

            var template = string.Format(@"<p style='font-family: Century Gothic, serif; font-size: 14px;'>Dear Mr/Ms Recipient,</p>
            <p style='font-family: Century Gothic, serif; font-size: 14px;'><b>{0}</b> has modified his/her requested <b>{1}</b>.</p>
            <p style='font-family: Century Gothic, serif; font-size: 14px;'>You can track its status through our ERP system by clicking <a href='{0}login'>here</a>.</p>
            <br/>
            <p style='font-family: Century Gothic, serif; font-size: 13px;'>Best Regrads</p>
            <p style='font-family: Century Gothic, serif; font-size: 13px;'><b>ProMatrix</b></p>
            <p style='font-family: Century Gothic, serif; font-size: 10px;color:#FF0000'>This is an auto-generated mail and should not be replied to.</p>", empDtls, leaveTypeName);
            return template;
        }
        public static string Cancelled(string empDtls, string leaveTypeName)
        {

            var template = string.Format(@"<p style='font-family: Century Gothic, serif; font-size: 14px;'>Dear Mr/Ms Recipient,</p>
            <p style='font-family: Century Gothic, serif; font-size: 14px;'><b>{0}</b> has cancelled his/her requested <b>{1}</b>.</p>
            <p style='font-family: Century Gothic, serif; font-size: 14px;'>You can track its status through our ERP system by clicking <a href='{0}login'>here</a>.</p>
            <br/>
            <p style='font-family: Century Gothic, serif; font-size: 13px;'>Best Regrads</p>
            <p style='font-family: Century Gothic, serif; font-size: 13px;'><b>ProMatrix</b></p>
            <p style='font-family: Century Gothic, serif; font-size: 10px;color:#FF0000'>This is an auto-generated mail and should not be replied to.</p>", empDtls, leaveTypeName);
            return template;
        }
        public static string Approved(string leaveTypeName, string userName, string remarks)
        {

            var template = string.Format(@"<p style='font-family: Century Gothic, serif; font-size: 14px;'>Dear Mr/Ms {0}</p>
            <p style='font-family: Century Gothic, serif; font-size: 14px;'><b>Your {1}</b> request has been approved.</p>
            <p style='font-family: Century Gothic, serif; font-size: 14px;'>You can track its status through our ERP system by clicking <a href='{0}login'>here</a>.</p>
            <br/>
            <p style='font-family: Century Gothic, serif; font-size: 13px;'>Best Regrads</p>
            <p style='font-family: Century Gothic, serif; font-size: 13px;'><b>ProMatrix</b></p>
            <p style='font-family: Century Gothic, serif; font-size: 10px;color:#FF0000'>This is an auto-generated mail and should not be replied to.</p>", userName, leaveTypeName);

            return template;
        }
        public static string Recheck(string leaveTypeName, string userName, string remarks)
        {
            return $@"<div style='font-family: Helvetica, Arial, sans-serif; min-width: 1000px; overflow: auto; line-height: 2;'>        
                <p style='font-size: 1.1em;'>Dear {userName},</p>
                <p>Your <b>{leaveTypeName}</b> request is requested to be re-examined with the following comment(s):</p>
                <p><b>{remarks}</b>.</p>    
                <br />
                <p style='font-size: 0.9em;'>Best Regards,<br />ProMatrix</p>
                <br />
                <p><span style='color: red;'>Note: This is a system-generated mail, please do not reply.</span></p>
            </div>";
        }
        public static string Cancelled(string leaveTypeName, string userName, string remarks)
        {
            return $@"<div style='font-family: Helvetica, Arial, sans-serif; min-width: 1000px; overflow: auto; line-height: 2;'>        
                <p style='font-size: 1.1em;'>Dear {userName},</p>
                <p>Your <b>{leaveTypeName}</b> request has been declined with the following comment(s):</p>
                <p><b>{remarks}</b>.</p>                   
                <br />
                <p style='font-size: 0.9em;'>Best Regards,<br />ProMatrix</p>
                <br />
                <p><span style='color: red;'>Note: This is a system-generated mail, please do not reply.</span></p>
            </div>";
        }

    }
}
