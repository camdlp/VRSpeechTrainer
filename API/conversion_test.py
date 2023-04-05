import os
from PIL import Image
from app import *

def save_images(images, output_folder):
    if not os.path.exists(output_folder):
        os.makedirs(output_folder)

    for i, image in enumerate(images):
        image.save(os.path.join(output_folder, f'image_{i}.png'), 'PNG')

if __name__ == '__main__':
    pptx_path = 'Slides\Oferta_Gachia.pptx'
    pdf_path = 'Slides\Informe_Grupo3B.pdf'

    pptx_images = convert_pptx_to_png(pptx_path)
    save_images(pptx_images, 'output/pptx_images')

    pdf_images = convert_pdf_to_png(pdf_path)
    save_images(pdf_images, 'output/pdf_images')
