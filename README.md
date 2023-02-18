# Deploying a .NET 6 API to Cloud Run using GitHub Actions

This guide will walk you through the steps to set up a GitHub Actions workflow to automatically build and deploy a .NET 6 API to Cloud Run.

## Prerequisites

- A GitHub account and a repository with your .NET 6 API code
- A Google Cloud project with billing enabled
- The [Google Cloud SDK](https://cloud.google.com/sdk/docs/install) installed on your local machine
- Basic knowledge of Docker and Dockerfiles

## Create a Service Account Key

1. In the Cloud Console, navigate to the **IAM & Admin** section and select **Service accounts**.
2. Select **Create service account**.
3. Enter a name and description for the service account.
4. Assign the **Cloud Run Invoker** and **Storage Object Creator** roles to the service account.
5. Create a key in JSON format and save it to your local machine.

## Configure GitHub Secrets

1. In your GitHub repository, go to **Settings > Secrets** and click **New repository secret**.
2. Create a new secret named `GCR_SERVICE_ACCOUNT_KEY` and copy the contents of the JSON key file you downloaded earlier.

## Write the GitHub Actions Workflow

1. In your GitHub repository, create a new directory named `.github/workflows`.
2. Create a new file in this directory named `cloud-run.yml`.
3. Paste the following code into the file:

```yaml
name: Build and Push Docker Image

on:
  push:
    branches: [master]

env:
  IMAGE_NAME: gcr.io/<your-project-id>/<your-image-name>

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
    - name: Checkout
      uses: actions/checkout@v2

    - name: Set up QEMU
      uses: docker/setup-qemu-action@v1

    - name: Set up Docker Buildx
      uses: docker/setup-buildx-action@v1

    - name: Login to Google Container Registry
      uses: google-github-actions/setup-gcloud@master
      with:
        project_id: <your-project-id>
        service_account_key: ${{ secrets.GCR_SERVICE_ACCOUNT_KEY }}
      id: gcloud

    - name: Build and Push
      id: docker_build
      uses: docker/build-push-action@v2
      with:
        context: .
        push: true
        tags: ${{ env.IMAGE_NAME }}:${{ github.sha }}

    - name: Image Digest
      run: echo ${{ steps.docker_build.outputs.digest }}
```

4. Replace `your-project-id`, `your-image-name`, and `your-service-name` with your own project ID, image name, and service name.

## Update Your Dockerfile

Update your Dockerfile to copy the build output to the `app` directory in the container. Here's an example Dockerfile:

```bash
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "ConfigurationApi.dll"]
```

## Conclusion
That's it! You've successfully set up GitHub Actions to build and push a Docker image to Google Container Registry. You can now use the Docker image in your Google Cloud deployments, or pull it down to your local machine for development and testing.