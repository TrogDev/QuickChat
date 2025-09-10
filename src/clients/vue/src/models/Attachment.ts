import type { AttachmentType } from './enums/AttachmentType';

export default interface Attachment {
  id: string;
  fileName: string;
  attachmentType: AttachmentType;
  url: string;
  size: number;
}
