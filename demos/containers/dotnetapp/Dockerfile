FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app
COPY . /app
# install nodejs and thus npm
RUN apt-get update -y && curl -sL https://deb.nodesource.com/setup_10.x | bash --debug && apt-get install nodejs -yq
RUN dotnet publish -c Release -r linux-musl-x64 -o out 

FROM mcr.microsoft.com/dotnet/core/runtime-deps:3.0-alpine
WORKDIR /app
COPY --from=build /app/out ./
ENV ASPNETCORE_ENVIRONMENT=Production
#ENV ASPNETCORE_URLS=https://+:5000
# EXPOSE 5001/tcp
# EXPOSE 5000/tcp
ENTRYPOINT [ "./app" ]