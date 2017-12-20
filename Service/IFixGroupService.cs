using System.Collections.Generic;
using Xmu.Crms.Shared.Models;

namespace Xmu.Crms.Shared.Service
{
    public interface IFixGroupService
    {
        /// <summary>
        /// 按FixGroupId删除FixGroupMember.
        /// @author zhouzhongjun
        /// </summary>
        /// <param name="fixGroupId">固定分组Id</param>
        /// <returns>true删除成功  false删除失败</returns>
        /// <exception cref="ArgumentException">id格式错误</exception>
        /// <exception cref="FixGroupNotFoundException">id格式错误</exception>
        bool DeleteFixGroupMemberByFixGroupId(long fixGroupId);

        /// <summary>
        /// 将学生加入固定小组.
        /// @author YeHongjie
        /// </summary>
        /// <param name="userId">学生的id</param>
        /// <param name="groupId">要加入固定小组的id</param>
        /// <returns>long 若创建成功返回该条记录的id，失败则返回-1</returns>
        /// <exception cref="ArgumentException">id格式错误</exception>
        /// <exception cref="UserNotFoundException">不存在该学生</exception>
        /// <exception cref="FixGroupNotFoundException">未找到小组</exception>
        /// <exception cref="InvalidOperationException">待添加学生已经在小组里了</exception>
        long InsertFixGroupMemberById(long userId, long groupId);

        /// <summary>
        /// 查询固定小组成员.
        /// @author YeHongjie
        /// </summary>
        /// <param name="groupId">要查询的固定小组id</param>
        /// <returns>List 固定小组成员信息</returns>
        /// <exception cref="ArgumentException">id格式错误</exception>
        /// <exception cref="FixGroupNotFoundException">未找到小组</exception>
        List<UserInfo> ListFixGroupMemberByGroupId(long groupId);

        /// <summary>
        /// 按classId查找FixGroup信息.
        /// @author zhouzhongjun
        /// </summary>
        /// <param name="classId">班级Id</param>
        /// <returns>null 固定分组列表</returns>
        /// <exception cref="ArgumentException">id格式错误</exception>
        List<FixGroup> ListFixGroupByClassId(long classId);

        /// <summary>
        /// 按classId删除FixGroup
        /// @author zhouzhongjun
        /// </summary>
        /// 
        /// 先根据classId得到所有的FixGroup信息，再根据FixGroupid删除FixGroupMember表的信息，最后再将FixGroup信息删除
        /// 
        /// <param name="classId">班级Id</param>
        /// <returns>true删除成功  false删除失败</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IFixGroupService.ListFixGroupByClassId(System.Int64)"/>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IFixGroupService.DeleteFixGroupByGroupId(System.Int64)"/>
        /// <exception cref="ArgumentException">id格式错误</exception>
        /// <exception cref="ClassNotFoundException">未找到班级</exception>
        bool DeleteFixGroupByClassId(long classId);

        /// <summary>
        /// 删除固定小组.
        /// @author YeHongjie
        /// </summary>
        /// <param name="groupId">固定小组的id</param>
        /// <returns>state 若删除成功返回true，失败返回false</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IFixGroupService.DeleteFixGroupMemberByFixGroupId(System.Int64)"/>
        /// <exception cref="ArgumentException">id格式错误</exception>
        /// <exception cref="FixGroupNotFoundException">未找到小组</exception>
        bool DeleteFixGroupByGroupId(long groupId);

        /// <summary>
        /// 修改固定小组.
        /// @author YeHongjie
        /// </summary>
        /// 
        /// 不包括成员
        /// 
        /// <param name="groupId">小组的id</param>
        /// <param name="fixGroupBo">小组信息</param>
        /// <returns>Boolean 若更新成功返回true，失败返回false</returns>
        /// <exception cref="ArgumentException">id格式错误</exception>
        /// <exception cref="FixGroupNotFoundException">未找到小组</exception>
        bool UpdateFixGroupByGroupId(long groupId, FixGroup fixGroupBo);

        /// <summary>
        /// 查询固定小组.
        /// @author YeHongjie
        /// </summary>
        /// 
        /// 按照id查询某一固定小组的信息（包括成员）
        /// 
        /// <param name="groupId">小组的id</param>
        /// <returns>fixGroupBO 固定小组对象，若未找到相关小组返回空(null)</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IFixGroupService.ListFixGroupMemberByGroupId(System.Int64)"/>
        /// <exception cref="ArgumentException">id格式错误</exception>
        /// <exception cref="FixGroupNotFoundException">未找到小组</exception>
        FixGroup GetFixGroupByGroupId(long groupId);

        /// <summary>
        /// 小组取消话题.
        /// @author heqi
        /// </summary>
        /// 
        /// 小组按小组id取消讨论课的话题的选择
        /// 
        /// <param name="groupId">小组id</param>
        /// <returns>true or false 返回取消话题是否成功</returns>
        /// <exception cref="ArgumentException">id格式错误</exception>
        /// <exception cref="FixGroupNotFoundException">未找到小组</exception>
        bool DeleteTopicByGroupId(long groupId);

        /// <summary>
        /// 按id获取小组.
        /// @author heqi
        /// </summary>
        /// 
        /// 通过学生id和班级id获取学生所在的班级固定小组
        /// 
        /// <param name="userId">学生id</param>
        /// <param name="classId">班级id</param>
        /// <returns>GroupBO 返回班级固定小组的信息</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.IUserService.GetUserByUserId(System.Int64)"/>
        /// <exception cref="ArgumentException">id格式错误</exception>
        /// <exception cref="ClassNotFoundException">未找到班级</exception>
        /// <exception cref="UserNotFoundException">不存在该学生</exception>
        FixGroup GetFixedGroupById(long userId, long classId);

        /// <summary>
        /// 根据groupId修改group.
        /// @author aixing
        /// </summary>
        /// <param name="groupId">要修改的group的Id</param>
        /// <param name="group">新的group信息</param>
        /// <returns>Boolean 若更新成功返回true，失败返回false</returns>
        /// <exception cref="ArgumentException">id格式错误</exception>
        /// <exception cref="FixGroupNotFoundException">未找到小组</exception>
        bool UpdateSeminarGroupById(long groupId, SeminarGroup group);
    }
}