ALTER TABLE [dbo].[Entities]  WITH CHECK ADD  CONSTRAINT [FK_Entities_Clients] FOREIGN KEY([client_fid])
REFERENCES [dbo].[Clients] ([client_pid])
GO

ALTER TABLE [dbo].[Entities] CHECK CONSTRAINT [FK_Entities_Clients]
GO