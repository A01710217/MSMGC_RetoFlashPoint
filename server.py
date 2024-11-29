#servidor
from http.server import BaseHTTPRequestHandler, HTTPServer
import subprocess
import os
import json

class Server(BaseHTTPRequestHandler):

    def _set_response(self):
        self.send_response(200)
        self.send_header('Content-type', 'application/json')
        self.end_headers()

    def do_POST(self):
        try:
            # Ejecutar el notebook usando nbconvert
            current_dir = os.path.dirname(os.path.abspath(__file__))
            notebook_path = os.path.join(current_dir, "FlashPoint.ipynb")
            subprocess.run(
                ["jupyter", "nbconvert", "--to", "notebook", "--execute", notebook_path],
                check=True
            )

            # Leer el archivo JSON generado
            json_path = os.path.join(current_dir, "combined_config_animation.json")
            with open(json_path, "r") as json_file:
                animation_data = json_file.read()

            # Enviar el archivo JSON al cliente
            self._set_response()
            self.wfile.write(animation_data.encode('utf-8'))

        except Exception as e:
            self.send_response(500)
            self.end_headers()
            self.wfile.write(f"Error ejecutando el notebook: {e}".encode('utf-8'))

def run_server(server_class=HTTPServer, handler_class=Server, port=8585):
    server_address = ('', port)
    httpd = server_class(server_address, handler_class)
    print("Starting server...")
    try:
        httpd.serve_forever()
    except KeyboardInterrupt:
        pass
    httpd.server_close()
    print("Stopping server...")

if __name__ == "__main__":
    run_server()
