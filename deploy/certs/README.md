# TLS-Zertifikate für ubahn.local

Die Zertifikate liegen **nicht** im Repository (sie werden lokal mit mkcert
erzeugt). Auf der vmLP1 ist mkcert installiert und die Root-CA bereits im
System-Trust (`mkcert -install` wurde ausgeführt).

Zertifikat erzeugen (aus diesem Verzeichnis):

```bash
cd deploy/certs
mkcert ubahn.local
```

Das erzeugt:

- `ubahn.local.pem`      (Zertifikat)
- `ubahn.local-key.pem`  (privater Schlüssel)

Genau diese Dateinamen erwartet der `Caddyfile`. Danach den Stack starten:

```bash
cd ../..
docker compose up -d
```
