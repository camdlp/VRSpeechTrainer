from flask import Flask, jsonify, request, send_file, render_template
import os
import io
import zipfile
from PyPDF2 import PdfFileReader
from PIL import Image
import tempfile
from pdf2image import convert_from_path

app = Flask(__name__)
app.config['UPLOAD_FOLDER'] = 'uploads/'

###### CONVERSIONS ######

def convert_pdf_to_png(filepath):
    # Convert the PDF file to a list of images
    images = convert_from_path(filepath)

    # Return the images
    return images

###### ROUTES ######

@app.route('/')
def index():
    return render_template('index.html')

@app.route('/slides')
def get_slide_files():
    folder_path = './Slides'
    files = os.listdir(folder_path)
    return jsonify(files)

@app.route('/recordings')
def get_recording_files():
    folder_path = './Recordings'
    files = os.listdir(folder_path)
    return jsonify(files)

@app.route('/api/download_slides')
def download_slides():
    filename = request.args.get('filename')
    filepath = os.path.join('Slides', filename)

    if os.path.isfile(filepath):
        png_images = []
        try:
            ext = os.path.splitext(filepath)[1].lower()
            if ext == '.pdf':
                png_images = convert_pdf_to_png(filepath)
            else:
                return 'Invalid file format', 400
        except ValueError:
            return 'Invalid file format', 400
        
        zip_filename = os.path.splitext(filename)[0] + '.zip'
        with zipfile.ZipFile(zip_filename, 'w') as zip_file:
            for i in range(len(png_images)):
                with tempfile.NamedTemporaryFile(suffix='.png', delete=False) as temp_file:
                    png_images[i].save(temp_file, format='png')
                    temp_file.close()  # Close the temp file before deleting it
                    zip_file.write(temp_file.name, f'slide_{i}.png')
                    os.unlink(temp_file.name)
                
        return send_file(zip_filename, as_attachment=True)
    else:
        return 'File not found', 404

@app.route('/api/download_recording')
def download_recording():
    filename = request.args.get('filename')
    filepath = os.path.join('Recordings', filename)

    if os.path.isfile(filepath):
        return send_file(filepath, as_attachment=True)
    else:
        return 'File not found', 404

@app.route('/api/upload', methods=['POST'])
def upload_file():
    file = request.files['file']
    if file:
        file_ext = os.path.splitext(file.filename)[1].lower()

        if file_ext in ('.pdf'):
            type = 'Slides'
        elif file_ext == '.mp4':
            type = 'Recordings'
        else:
            return 'File type not supported', 400

        upload_dir = type
        if not os.path.isdir(upload_dir):
            os.makedirs(upload_dir)
        file.save(os.path.join(upload_dir, file.filename))
        return 'File uploaded successfully'
    else:
        return 'No file uploaded', 400
    
if __name__ == '__main__':
    app.run(debug=True)
