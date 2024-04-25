variable "TAGS" {
  default = "latest"
}
variable "DIRECTORY" {
  default = "ghcr.io"
}

variable "platforms" {
  default = ["windows/amd64", "linux/amd64"]
}

function "tagImage" {
  params = [image]
  result = [for tag in split(",", TAGS) : "${DIRECTORY}/${image}:${tag}"]
}

group "default" {
  targets = [
    "unity-docker-build",
  ]
}

target "unity-docker-build" {
  dockerfile = "unity-docker-build/Dockerfile"
  tag = tagImage("unity-docker-build")
  secret = [
    "type=env,id=UNITY_USERNAME",
    "type=env,id=UNITY_PASSWORD",
    "type=env,id=UNITY_LICENSE",
  ]
  platforms = platforms
}