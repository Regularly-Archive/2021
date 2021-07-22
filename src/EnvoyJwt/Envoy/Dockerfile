FROM envoyproxy/envoy-alpine:v1.16-latest
COPY ./envoy.yaml /etc/envoy.yaml
COPY ./certs/ /etc/envoy/certs/

RUN chmod go+r /etc/envoy.yaml
CMD ["/usr/local/bin/envoy", "-c", "/etc/envoy.yaml", "--service-cluster", "reverse-proxy"]
