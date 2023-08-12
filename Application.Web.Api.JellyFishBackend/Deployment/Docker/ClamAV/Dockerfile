# Verwende das offizielle Alpine Linux-Basisimage
FROM alpine:latest

# Aktualisiere das System und installiere ClamAV
RUN apk update && \
    apk add --no-cache clamav clamav-libunrar

# Aktualisiere die ClamAV-Virendefinitionen
RUN freshclam

# Expose die Ports 3310 und 7357
EXPOSE 3310 7357

# Starte den ClamAV-Daemon
CMD ["clamd", "--foreground"]