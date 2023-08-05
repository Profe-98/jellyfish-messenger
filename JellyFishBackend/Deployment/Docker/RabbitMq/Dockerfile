# Verwende das offizielle RabbitMQ-Basisimage aus dem Docker Hub
FROM rabbitmq:latest

# Kopiere die angepasste Konfigurationsdatei in den Container
COPY rabbitmq.conf /etc/rabbitmq/

# Setze die Umgebungsvariablen für RabbitMQ, um den Benutzer und den virtuellen Host zu erstellen
ENV RABBITMQ_DEFAULT_USER=jellyfish
ENV RABBITMQ_DEFAULT_PASS=admin1234
ENV RABBITMQ_DEFAULT_VHOST=webapi

EXPOSE 5672
EXPOSE 15672

# Optional: Führe zusätzliche Befehle während der Containererstellung aus
# Beispiel: Wenn du Plugins installieren möchtest
# RUN rabbitmq-plugins enable rabbitmq_management

# CMD ist in der offiziellen RabbitMQ-Basisimage definiert und startet den RabbitMQ-Server