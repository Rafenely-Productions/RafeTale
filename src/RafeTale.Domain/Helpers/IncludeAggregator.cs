using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace RafeTale.Domain.Helpers;

public class IncludeAggregator<T> where T : class
{
    public List<string> IncludePaths { get; } = [];

    // 1. Include normal de primer nivel (ej: r => r.Subraces)
    public IncludeAggregator<T> Include<TProperty>(Expression<Func<T, TProperty>> expression)
    {
        IncludePaths.Add(ExtractPath(expression));
        return this;
    }

    // 2. Tu método insignia para colecciones anidadas (ej: r => r.Traits hacia t => t.Modifiers)
    public IncludeAggregator<T> IncludeCollection<TProperty, TSubProperty>(
        Expression<Func<T, IEnumerable<TProperty>>> collectionExpression,
        Expression<Func<TProperty, TSubProperty>> subPropertyExpression)
    {
        string parent = ExtractPath(collectionExpression);
        string child = ExtractPath(subPropertyExpression);

        // Esto genera "Traits.Modifiers" de forma limpia
        IncludePaths.Add($"{parent}.{child}");
        return this;
    }

    public IncludeAggregator<T> IncludeCollection<TProperty, TSubProperty, TGrandSubProperty>(
     Expression<Func<T, IEnumerable<TProperty>>> collectionExpression,
     Expression<Func<TProperty, IEnumerable<TSubProperty>>> subPropertyExpression,
     Expression<Func<TSubProperty, TGrandSubProperty>> grandPropertyExpression)
    {
        string parent = ExtractPath(collectionExpression);
        string child = ExtractPath(subPropertyExpression);
        string grandchild = ExtractPath(grandPropertyExpression);

        // Esto genera de forma limpia: "Subclasses.Progressions.Features"
        IncludePaths.Add($"{parent}.{child}.{grandchild}");
        return this;
    }
    private static string ExtractPath(LambdaExpression expression)
    {
        var body = expression.Body;
        if (body is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
        {
            body = unary.Operand;
        }

        if (body is MethodCallExpression methodCall)
        {
            // Si el método es de LINQ (ej: Select), la propiedad real suele ser el primer argumento
            if (methodCall.Arguments.Count > 0)
            {
                var firstArg = methodCall.Arguments[0];
                if (firstArg is MemberExpression memberExpr)
                {
                    return memberExpr.Member.Name;
                }
            }
        }

        if (body is MemberExpression member)
        {
            return member.Member.Name;
        }

        var str = body.ToString();
        var selectIndex = str.IndexOf(".Select");
        if (selectIndex != -1)
        {
            str = str[..selectIndex];
        }

        var dotIndex = str.LastIndexOf('.');
        if (dotIndex != -1)
        {
            return str[(dotIndex + 1)..].Replace(")", "").Trim();
        }

        return str;
    }
}