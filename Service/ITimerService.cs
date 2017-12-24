using System;
using System.Collections.Generic;

namespace Xmu.Crms.Shared.Service
{
    /// <summary>
    /// 定时器
    /// @author qinlingyun liuaiqi
    /// @version 2.00
    /// </summary>
    public interface ITimerService
    {
        ///<summary>
        ///向Event表插入数据.
        ///@author qinlingyun
        /// </summary>
        /// <param name="time">事件的时间</param>
        /// <param name="className">类名称</param>
        /// <param name="methodName">方法名称</param>
        /// <param name="parameters">方法参数</param>
        void InsertEvent(DateTime time, string className, string methodName, object[] parameters);

        ///<summary>
        ///更新Event表.
        ///@author qinlingyun
        /// </summary>
        /// <param name="eventId">事件的Id</param>
        /// <param name="time">事件的新时间</param>
        void UpdateEvent(long eventId, DateTime time);

        ///<summary>
        ///每十分钟检查一次Event实体的状况
        ///@author qinlingyun
        /// </summary>
        void Scheduled();
    }
}