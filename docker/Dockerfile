FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

ARG TS3_AUDIOBOT_FLAVOUR="develop"

# Disable the invariant mode (set in base image)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENV PUID=1000
ENV PGID=1000

COPY ./entrypoint.sh /entrypoint.sh

# install all pre-requisites, these will be needed always
RUN apk --no-cache add \
    opus-dev \
    youtube-dl \
    yt-dlp \
    ffmpeg \
    icu-libs \
    su-exec

# download and install the TS3AudioBot in the specified version and flavour
RUN mkdir -p /app \
    && cd /app \
    && echo "downloading https://splamy.de/api/nightly/projects/ts3ab/${TS3_AUDIOBOT_FLAVOUR}/download" \
    && wget --no-check-certificate https://splamy.de/api/nightly/projects/ts3ab/${TS3_AUDIOBOT_FLAVOUR}/download -O TS3AudioBot.zip \
    && unzip TS3AudioBot.zip \
    && rm TS3AudioBot.zip

# make data directory
RUN mkdir -p /app/data

WORKDIR /app/data

# expose the webserver port
EXPOSE 58913

CMD ["/entrypoint.sh"]