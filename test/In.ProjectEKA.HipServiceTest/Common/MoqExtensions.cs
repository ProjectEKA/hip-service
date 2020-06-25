namespace In.ProjectEKA.HipServiceTest.Common
{
    using System;
    using System.Collections.Generic;
    using Moq.Language.Flow;

    public static class MoqExtensions
    {
        public static IReturnsResult<TMock> ReturnsInOrder<TMock, TResult>(this ISetup<TMock, TResult> setup,
            params Func<TResult>[] valueFunctions)
            where TMock : class
        {
            var queue = new Queue<Func<TResult>>(valueFunctions);
            return setup.Returns(() => queue.Dequeue()());
        }
    }
}