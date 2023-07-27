using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LLS.Common.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Permission
    {
        AddDeleteEdit_User,
        AddDeleteEdit_Course,
        AddDeleteEdit_Exp,
        AddDeleteEdit_Role,
        AssignExpToCourse,
        AssignUserToCourse,
        //RemoveExpFromCourse,
        //RemoveUserFromCourse,
        AssignRoleToUser,
        //GetAssignedExp_Student,
        SubmitAssignedExp_Student,
        GradeStudentAnswers_Teacher,
        //GetAssignedCourse_Student,
        //GetAssignedCourse_Teacher,
        //ResetUserPassword 

        //Views
        ViewUsers,
        ViewExp,
        ViewRoles,
        ViewCourses,

        ViewActiveLab_Teacher,
        ViewGradeBooks_Teacher,
        ViewAnalytics_Teacher,

        ViewAssignedExpCourse_Student

    }
}
