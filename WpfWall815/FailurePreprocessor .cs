using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfWall815
{

    public class FailurePreprocessor : IFailuresPreprocessor
    {
         public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
    {
        // 获取所有的失败/警告信息
        IList<FailureMessageAccessor> failures = failuresAccessor.GetFailureMessages();

        // 遍历所有警告并忽略它们
        foreach (FailureMessageAccessor failure in failures)
        {
            // 直接删除警告
            failuresAccessor.DeleteWarning(failure);
        }

        // 返回处理结果，指定继续处理事务
        return FailureProcessingResult.Continue;
    }
    }

}

