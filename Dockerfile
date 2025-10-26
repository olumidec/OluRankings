# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file first to leverage Docker layer caching
COPY OluRankings.csproj ./
RUN dotnet restore ./OluRankings.csproj

# Now copy the rest of your source and publish
COPY . .
RUN dotnet publish ./OluRankings.csproj -c Release -o /out /p:UseAppHost=false

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /out .

# Render sets $PORT (defaults to 10000). Bind Kestrel to it.
# Using bash ensures ${PORT} is expanded at runtime.
EXPOSE 10000
ENV ASPNETCORE_ENVIRONMENT=Production
CMD ["bash","-c","dotnet OluRankings.dll --urls http://0.0.0.0:${PORT:-10000}"]
