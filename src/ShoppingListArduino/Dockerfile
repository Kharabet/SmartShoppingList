FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 5000

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["ShoppingListArduino/ShoppingListArduino.csproj", "ShoppingListArduino/"]
RUN dotnet restore "ShoppingListArduino/ShoppingListArduino.csproj"
COPY . .
WORKDIR "/src/ShoppingListArduino"
RUN dotnet build "ShoppingListArduino.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ShoppingListArduino.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ShoppingListArduino.dll"]