#!/bin/ash

USER=ts3bot

EXEC_PREFIX=""

if [ "$PUID" != "0" ] && [ "$PGID" != "0" ]; then
    addgroup --gid "${PGID}" "${USER}"
    echo "INFO: Added group ${USER} with PGID ${PUID}"

    adduser --disabled-password --uid "${PUID}" --ingroup "${USER}" "${USER}"
    echo "INFO: Added user ${USER} with PUID ${PUID}"

    chown -R ${USER}:${USER} /app/
    echo "INFO: Set permissions in /app for ${USER}:${USER}"

    EXEC_PREFIX="su-exec ${USER}"
    EXEC_PREFIX="${EXEC_PREFIX} "
else
    echo "WARNING: running as root"
fi

${EXEC_PREFIX}dotnet /app/TS3AudioBot.dll --non-interactive