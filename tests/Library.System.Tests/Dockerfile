FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Library.Domain/Library.Domain.csproj", "src/Library.Domain/"]
COPY ["src/Library.Application/Library.Application.csproj", "src/Library.Application/"]
COPY ["src/Library.Infrastructure/Library.Infrastructure.csproj", "src/Library.Infrastructure/"]
COPY ["src/Library.Api/Library.Api.csproj", "src/Library.Api/"]
COPY ["tests/Library.System.Tests/Library.System.Tests.csproj", "tests/Library.System.Tests/"]

RUN dotnet restore "tests/Library.System.Tests/Library.System.Tests.csproj"

COPY . .
WORKDIR "/src/tests/Library.System.Tests"
RUN dotnet build "Library.System.Tests.csproj" -c Release

FROM build AS testrunner
ENTRYPOINT ["dotnet", "test", "--no-build", "-c", "Release", "--logger:console;verbosity=normal"]