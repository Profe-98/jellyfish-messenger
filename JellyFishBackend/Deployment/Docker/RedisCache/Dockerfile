# Verwende das offizielle Redis-Basisimage aus dem Docker Hub
FROM redis:latest

# Kopiere die angepasste Konfigurationsdatei in den Container
COPY redis.conf /usr/local/etc/redis/redis.conf

# Expose den Standard-Redis-Port (6379)
EXPOSE 6379

# CMD ist in der offiziellen Redis-Basisimage definiert und startet den Redis-Server
