using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XFramework.Entity
{
    /// <summary>
    /// XFramework自定义校验格式的正则枚举
    /// </summary>
    [Serializable]
    public enum RegexFormat
    { 
        /// <summary>
        /// 非负整数（正整数 + 0） ^\d+$
        /// </summary>
        Nonnegative = 0,
        /// <summary>
        /// 正整数 ^[0-9]*[1-9][0-9]*$
        /// </summary>
        Positive = 1,
        /// <summary>
        /// 非正整数 ^((-\d+)|(0+))$
        /// </summary>
        NegativePlusZero = 2,
        /// <summary>
        /// 负整数 -[0-9]*[1-9][0-9]*
        /// </summary>
        Negative = 3,
        /// <summary>
        /// 整数 ^-?\d+$
        /// </summary>
        Integer = 4,
        /// <summary>
        /// 非负浮点数（正浮点数 + 0）^\d+(\.\d+)?$
        /// </summary>
        PositiveFloatPlusZero = 5,
        /// <summary>
        /// 正浮点数 ^(([0-9]+\\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\\.[0-9]+)|([0-9]*[1-9][0-9]*))$ 
        /// </summary>
        PositiveFloat = 6,
        /// <summary>
        /// 非正浮点数（负浮点数 + 0） ^((-\\d+(\\.\\d+)?)|(0+(\\.0+)?))$
        /// </summary>
        NegativeFloatPlusZero = 7,
        /// <summary>
        /// 负浮点数 ^(-(([0-9]+\\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\\.[0-9]+)|([0-9]*[1-9][0-9]*)))$ 
        /// </summary>
        NegativeFloat = 8,
        /// <summary>
        /// 浮点数 ^(-?\\d+)(\\.\\d+)?$ 
        /// </summary>
        Float = 9,
        /// <summary>
        /// 由26个英文字母组成的字符串 ^[A-Za-z0-9]+$
        /// </summary>
        EnglishChar = 10,
        /// <summary>
        /// 由26个英文字母的大写组成的字符串 ^[A-Z]+$
        /// </summary>
        EnglishCharLowerCase = 11,
        /// <summary>
        /// 由26个英文字母的小写组成的字符串 ^[a-z]+$
        /// </summary>
        EnglishCharUpCase = 12,
        /// <summary>
        /// 由数字和26个英文字母组成的字符串 ^[A-Za-z0-9]+$
        /// </summary>
        EnglishNumber = 13,
        /// <summary>
        /// 由数字、26个英文字母或者下划线组成的字符串 ^\\w+$
        /// </summary>
        EnglishNumberUnderline = 14,
        /// <summary>
        /// url ^[a-zA-z]+://(\\w+(-\\w+)*)(\\.(\\w+(-\\w+)*))*(\\?\\S*)?$
        /// </summary>
        URL = 15
    }
}
