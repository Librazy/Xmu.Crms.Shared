using System.Collections.Generic;

namespace Xmu.Crms.Shared.Service
{
    /**
 * 
 * @author Huhui QinLingyun ZhouZhongjun
 * @version 2.00
 *
 */
    public interface IGradeService
    {
        /// <summary>
        /// 按topicId删除学生打分表.
        /// @author zhouzhongjun
        /// </summary>
        /// <param name="topicId">话题Id</param>
        /// <returns>true删除成功  false删除失败</returns>
        bool DeleteStudentScoreGroupByTopicId(long topicId);

        /// <summary>
        /// 获取某学生所有讨论课的成绩.
        /// @author qinlingyun
        /// </summary>
        /// <param name="userId">学生id</param>
        /// <param name="seminarGroupId">讨论课小组id</param>
        /// <returns>list 讨论课分数列表</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarGroupService.ListSeminarGroupIdByStudentId(System.Int64)"/>
        List<long> ListSeminarGradeBySeminarGroupId(long userId, long seminarGroupId);

        /// <summary>
        /// 提交对其他小组的打分.
        /// @author Huhui
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="seminarId">讨论课Id</param>
        /// <param name="groupId">小组Id</param>
        /// <param name="grade">分数</param>
        /// <returns>true 提交成功 false 提交失败</returns>
        bool InsertGroupGradeByUserId(long userId, long seminarId, long groupId, long grade);

        /// <summary>
        /// 按ID设置小组报告分.
        /// @author Huhui
        /// </summary>
        /// <param name="seminarGroupId">讨论课组id</param>
        /// <param name="grade">分数</param>
        /// <returns>Boolean true 操作成功 false 操作失败</returns>
        bool UpdateGroupByGroupId(long seminarGroupId, long grade);

        /// <summary>
        /// 获取某学生的讨论课成绩列表.
        /// @author qinlingyun
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns>list 讨论课成绩列表</returns>
        /// <seealso cref="M:Xmu.Crms.Shared.Service.ISeminarGroupService.ListSeminarGroupBySeminarId(System.Int64)"/>
        List<long> ListSeminarGradeByStudentId(long userId);
    }
}