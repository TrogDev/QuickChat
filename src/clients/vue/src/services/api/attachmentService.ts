import type Attachment from '@/models/Attachment';
import { apiClient } from './client';
import type { AttachmentType } from '@/models/enums/AttachmentType';
import type { AxiosProgressEvent } from 'axios';

export async function upload(
  file: File,
  type: AttachmentType,
  progressCallback?: (loaded: number, total: number | undefined) => void,
): Promise<Attachment> {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('type', type);

  const { data } = await apiClient.post<Attachment>('/attachments', formData, {
    onUploadProgress: (progressEvent: AxiosProgressEvent) =>
      progressCallback?.(progressEvent.loaded, progressEvent.total),
  });

  return data;
}
