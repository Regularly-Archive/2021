FROM envoyproxy/envoy-alpine:v1.16-latest
COPY ./envoy.yaml /etc/envoy.yaml
RUN mkdir /etc/descriptor/
COPY ./descriptor/ /etc/descriptor/
RUN chmod go+r /etc/descriptor
CMD ["/usr/local/bin/envoy", "-c", "/etc/envoy.yaml", "--service-cluster", "reverse-proxy"]
