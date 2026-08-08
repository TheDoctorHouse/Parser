namespace TheParser;

public static class ExpressionCalculator
{
    public static double Evaluate(Expr ex)
    {
        if (ex is NumberExpression ne)
            return ne.Value;
        if (ex is UnaryExpression ue)
        {
            var value = Evaluate(ue.Expr);
            return ue.Operator switch
            {
                TokenType.Plus => value,
                TokenType.Minus => -value,
                _ => throw new ArgumentOutOfRangeException(nameof(ex))
            };
        }
        if (ex is BinaryExpression be)
        {
            var left = Evaluate(be.Left);
            var right = Evaluate(be.Right);
            return be.Operator switch
            {
                TokenType.Plus => left + right,
                TokenType.Minus => left - right,
                TokenType.Multiply => left * right,
                TokenType.Divide => left / right,
                _ => throw new ArgumentOutOfRangeException(nameof(ex)),
            };
        }

        throw new NotImplementedException();
    }
}