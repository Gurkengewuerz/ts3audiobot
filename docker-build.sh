#!/bin/bash

REGISTRY="docker.io"
USERNAME="gurken2108"
PROJECT="ts3audiobot"

docker build -t ${USERNAME}/${PROJECT}:latest docker/
RESULT=$?
if [ $RESULT -eq 0 ]; then
  echo Docker build success
else
  echo Docker build failed. Please check the logs.
  exit 1
fi

docker tag ${USERNAME}/${PROJECT}:latest ${REGISTRY}/${USERNAME}/${PROJECT}:latest

docker push ${REGISTRY}/${USERNAME}/${PROJECT}
