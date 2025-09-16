set -euo pipefail

export VASILI_REST_CUDA=
#export VASILI_REST_CUDA="-cuda"

export VASILI_REST_NAME=vasili-rest
export VASILI_REST_VERSION=0.0.7
export VASILI_REST_LOG_LEVEL=DEBUG
export VASILI_REST_MODEL_CACHE_SIZE=3
export DOCKER_REGISTRY="docker.texttechnologylab.org/"


docker build \
  --build-arg VASILI_REST_NAME \
  --build-arg VASILI_REST_VERSION \
  --build-arg VASILI_REST_LOG_LEVEL \
  -t ${DOCKER_REGISTRY}${VASILI_REST_NAME}:${VASILI_REST_VERSION}${VASILI_REST_CUDA}\
  -f Dockerfile${VASILI_REST_CUDA} \
  .

docker tag \
  ${DOCKER_REGISTRY}${VASILI_REST_NAME}:${VASILI_REST_VERSION} \
  ${DOCKER_REGISTRY}${VASILI_REST_NAME}:latest