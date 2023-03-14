# Empty AWS Serverless Application Project

## Here are some steps to follow to get started from the command line:

Once you have edited your template and code you can deploy your application using the [Amazon.Lambda.Tools Global Tool](https://github.com/aws/aws-extensions-for-dotnet-cli#aws-lambda-amazonlambdatools) from the command line.

Install Amazon.Lambda.Tools Global Tools if not already installed.

```
    dotnet tool install -g Amazon.Lambda.Tools
```

If already installed check if new version is available.

```
    dotnet tool update -g Amazon.Lambda.Tools
```

Deploy application

```
    cd "ePollApplication/service/serverless"
    dotnet lambda deploy-serverless --region eu-west-1 --stack-name ePollApplication --s3-bucket epollapplication
```

## command to deploy : dotnet lambda deploy-serverless --region eu-west-1 --stack-name ePollApplication --s3-bucket epollapplication
