using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Library.Service.Interceptors;

public class GlobalExceptionInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"Validation Error: {ex.Message}");

            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Internal Error: {ex}");

            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }
}