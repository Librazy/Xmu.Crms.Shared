using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.Shared.Service
{
    /**
 * @author ModuleStandardGroup/YeHongjie
 * @version 2.00
 */
    public interface ILoginService
    {
        /// <summary>
        /// 微信登录.
        /// @author qinlingyun
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="code">微信小程序/OAuth2授权的Code</param>
        /// <param name="state">微信OAuth2授权的state。对于小程序，值恒为 MiniProgram</param>
        /// <param name="successUrl">微信OAuth2授权后跳转到的网址</param>
        /// <returns>user 该用户信息</returns>
        UserInfo SignInWeChat(long userId, string code, string state, string successUrl);


        /// <summary>
        /// 手机号登录.
        /// @author qinlingyun
        /// </summary>
        /// 
        /// User中只有phone和password，用于判断用户名密码是否正确
        /// 
        /// <param name="user">用户信息(手机号Phone和密码Password)</param>
        /// <returns>user 该用户信息</returns>
        UserInfo SignInPhone(UserInfo user);
    }
}